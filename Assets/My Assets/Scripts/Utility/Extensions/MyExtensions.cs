using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MyExtensions
{
    public static void GetCylinderPoints(this Transform t, Vector3 center, float height, float radius, out Vector3 p1, out Vector3 p2)
    {
        Vector3 centerWorldPos = t.TransformPoint(center);
        float cylinderLength = Mathf.Max(0, height * 0.5f - radius);
        p1 = centerWorldPos + Vector3.up * cylinderLength;
        p2 = centerWorldPos - Vector3.up * cylinderLength;
    }

    public static float PerceptualDecibelsToVolume(float dbValue, float minDb = -80f)
    {
        dbValue = Mathf.Clamp(dbValue, minDb, 0f);

        // Convert dB to linear gain
        float gain = Mathf.Pow(10f, dbValue / 20f);

        // Calculate min gain
        float minGain = Mathf.Pow(10f, minDb / 20f);

        // Inverse lerp on the gain scale
        float volume = Mathf.InverseLerp(minGain, 1f, gain);
        return volume;
    }

    public static float VolumeToPerceptualDecibels(float volume, float minDb = -80f)
    {
        volume = Mathf.Clamp01(volume);

        // 1. Define the minimum gain corresponding to your min dB
        float minGain = Mathf.Pow(10f, minDb / 20f); // e.g., -80 dB → ~0.0001 gain

        // 2. Interpolate on gain scale (not dB)
        float gain = Mathf.Lerp(minGain, 1f, volume);

        // 3. Convert back to decibels
        float db = 20f * Mathf.Log10(gain);
        return db;
    }

    public static Vector3 RemovePitch(this Vector3 direction)
    {
        return new Vector3(direction.x, 0f, direction.z).normalized;
    }

    public static T GetRandomDifferent<T>(this List<T> list, T previous)
    {
        // Filter out items equal to previous
        var options = list.Where(x => !EqualityComparer<T>.Default.Equals(x, previous)).ToList();

        if (options.Count == 0)
        {
            Debug.LogWarning("GetRandomDifferent: No different element available to choose");
            return previous;
        }

        int index = UnityEngine.Random.Range(0, options.Count);
        return options[index];
    }

    /// <summary>
    /// Returns normalized direction
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="ignorePitch">Remove pitch from normalized vector</param>
    /// <param name="forwardInfluence">0 = pure direction, 1 = pure source forward</param>
    /// <returns></returns>
    public static Vector3 DirectionTo(this Transform source, Component target, bool ignorePitch = false, float forwardInfluence = 0f)
    {
        Vector3 dir = (target.transform.position - source.position).normalized;
        if (ignorePitch) dir = new Vector3(dir.x, 0f, dir.z).normalized;
        if (forwardInfluence > 0) dir = Vector3.Slerp(dir, source.forward, forwardInfluence);
        return dir;
    }

    public static Vector3 DirectionTo(this Vector3 source, Vector3 target, bool ignorePitch = false)
    {
        Vector3 dir = (target - source).normalized;
        if (ignorePitch) dir = new Vector3(dir.x, 0f, dir.z).normalized;
        return dir;
    }

    public static float Remap(this float num, float minInput, float maxInput, float minOutput, float maxOutput)
    {
        return minOutput + (num - minInput) * (maxOutput - minOutput) / (maxInput - minInput);
    }

    public static Vector3 GetVector(this ITCAxis axis)
    {
        if (axis == ITCAxis.X)
        {
            return Vector3.right;
        }

        if (axis == ITCAxis.Y)
        {
            return Vector3.up;
        }

        if (axis == ITCAxis.Z)
        {
            return Vector3.forward;
        }

        if (axis == ITCAxis.NegX)
        {
            return -Vector3.right;
        }

        if (axis == ITCAxis.NegY)
        {
            return -Vector3.up;
        }

        return -Vector3.forward;
    }

    public static void SetLayerRecursive(this Transform transform, int layer, Transform except = null)
    {
        if (layer < 0) return;

        if (!transform || transform == except) return;

        transform.gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerRecursive(transform.GetChild(i), layer, except);
        }
    }

    public static IEnumerator FlashLineRenderer(this LineRenderer lineRenderer, float rateTarget, float duration, float lowTarget,
        float highTarget)
    {
        var startColor = lineRenderer.startColor;
        float flashRate = 0f;
        var newAlpha = highTarget;
        bool reachedLowTarget = false;
        var startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            if (!reachedLowTarget && newAlpha > lowTarget)
            {
                newAlpha -= flashRate * Time.deltaTime;
            }
            else
            {
                reachedLowTarget = true;
                newAlpha += flashRate * Time.deltaTime;

                if (newAlpha >= highTarget)
                {
                    reachedLowTarget = false;
                    // Debug.Log("Return to low target");
                }
            }

            var color = lineRenderer.startColor;
            lineRenderer.startColor = new Color(color.r, color.g, color.b, newAlpha);
            lineRenderer.endColor = new Color(color.r, color.g, color.b, newAlpha);

            flashRate = Mathf.Lerp(flashRate, rateTarget, Time.deltaTime / duration);

            yield return null;
        }

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = startColor;
    }

    public static bool HasParameter(this Animator anim, AnimatorControllerParameterType paramType, string paramName)
    {
        return anim && anim.parameters.Any(animParameter => animParameter.type == paramType && animParameter.name.Equals(paramName));
    }

    /// <summary>
    /// Ignore collisions between these colliders
    /// </summary>
    public static void IgnoreCollisionWithinList(this List<Collider> colliders)
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            for (int j = i + 1; j < colliders.Count; j++)
            {
                Physics.IgnoreCollision(colliders[i], colliders[j], true);
                // Debug.Log($"{colliders[i]} ignoring {colliders[j]}");
            }
        }
    }
    
    /// <summary>
    /// Safely disables a component if it supports being disabled.
    /// Works for Behaviours, Renderers, and Colliders.
    /// </summary>
    public static void DisableComponent(this Component component)
    {
        if (component is Behaviour behaviour)
        {
            behaviour.enabled = false;
        }
        else if (component is Renderer renderer)
        {
            renderer.enabled = false;
        }
        else if (component is Collider collider)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogWarning($"{component.name} of type {component.GetType().Name} cannot be disabled automatically.");
        }
    }
    
    public static bool ContainsIndex(this Array array, int index, int dimension)
    {
        if (index < 0)
            return false;

        return index < array.GetLength(dimension);
    }
}