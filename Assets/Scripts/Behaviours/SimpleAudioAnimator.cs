using System.Collections;
using System.Threading;
using Sevakon.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Animates the scale of a transform and optionally the emission color of a material with audio volume
    /// </summary>
    public class SimpleAudioAnimator : MonoBehaviour
    {
        /// <summary>
        /// A job to create an animation curve from audio data
        /// </summary>
        [BurstCompile]
        private struct MusicToAnimationCurveJob : IJob
        {
            [ReadOnly] public NativeArray<float> samples;

            public NativeList<float> volumeList;

            [WriteOnly] public NativeList<float> timeList;

            [ReadOnly] public float stepInSeconds;

            [ReadOnly] public float lengthInSeconds;

            public float minDetectedVolume;
            public float maxDetectedVolume;

            [ReadOnly] public float minScale;
            [ReadOnly] public float maxScale;

            public void Execute()
            {
                int sampleStep = (int) ((stepInSeconds / lengthInSeconds) * samples.Length);

                for (int i = 0; i < samples.Length; i += sampleStep)
                {
                    var len = math.min(samples.Length - i, sampleStep);

                    var slice = new NativeSlice<float>(samples, i, len);
                    float volume = 0;

                    for (int j = 0; j < slice.Length; j++)
                    {
                        volume += math.abs(slice[j]);
                    }

                    volume /= slice.Length;

                    if (volume > maxDetectedVolume)
                        maxDetectedVolume = volume;

                    if (volume < minDetectedVolume)
                        minDetectedVolume = volume;

                    timeList.Add((i / (float) samples.Length) * lengthInSeconds);
                    volumeList.Add(volume);
                }

                for (int i = 0; i < volumeList.Length; i++)
                {
                    volumeList[i] = LinearUtils.LerpTransform(minDetectedVolume, maxDetectedVolume, volumeList[i],
                        minScale, maxScale);
                }
            }
        }

        /// <summary>
        /// Audio source to read data from
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Minimum scale that corresponds to the lowest audio volume
        /// </summary>
        public float minScale = 1f;
        /// <summary>
        /// Maximum scale that corresponds to the loudest audio volume
        /// </summary>
        public float maxScale = 1.5f;

        /// <summary>
        /// The interval of keyframes in the animation, in seconds
        /// </summary>
        public float animationPrecision = 0.1f;

        /// <summary>
        /// Whether or not to animate the emission (glow) of the material
        /// </summary>
        public bool animateEmission;

        private Color color;
        private Material material;
        private Vector3 initialScale;

        private AnimationCurve curve;

        private IEnumerator Start()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (animateEmission)
            {
                material = GetComponent<Renderer>().material;
                color = material.GetColor("_EmissionColor");
            }

            initialScale = transform.localScale;

            curve = CreateAnimationCurve(audioSource.clip, animationPrecision, minScale, maxScale);

            yield return null;
        }

        private AnimationCurve CreateAnimationCurve(AudioClip clip, float step, float minScale = 0f,
            float maxScale = 1f)
        {
            var samples = new float[clip.samples];
            clip.GetData(samples, 0);

            var jobLevel = new MusicToAnimationCurveJob
            {
                samples = new NativeArray<float>(samples, Allocator.Persistent),
                lengthInSeconds = clip.length,
                stepInSeconds = step,
                maxDetectedVolume = float.MinValue,
                minDetectedVolume = float.MaxValue,
                minScale = minScale,
                maxScale = maxScale,
                timeList = new NativeList<float>(Allocator.Persistent),
                volumeList = new NativeList<float>(Allocator.Persistent)
            };
            var jobLevelHandle = jobLevel.Schedule();
            jobLevelHandle.Complete();

            curve = new AnimationCurve();

            for (int i = 0; i < jobLevel.timeList.Length; i++)
            {
                curve.AddKey(jobLevel.timeList[i], jobLevel.volumeList[i]);
            }

            jobLevel.samples.Dispose();
            jobLevel.timeList.Dispose();
            jobLevel.volumeList.Dispose();

            return curve;
        }


        private void Update()
        {
            var value = curve.Evaluate(audioSource.time) * audioSource.volume;

            transform.localScale = initialScale * value;

            if (animateEmission)
            {
                value = LinearUtils.LerpTransform(minScale, maxScale, value, 1.5f, 6f);
                var glowColor = color * Mathf.LinearToGammaSpace(value);
                material.SetColor("_EmissionColor", glowColor);
            }
        }
    }
}