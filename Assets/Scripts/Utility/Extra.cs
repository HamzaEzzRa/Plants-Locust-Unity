using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct FloatRange
{
    [SerializeField] private float min, max;

    public float Min => min;

    public float Max => max;

    public float RandomValueInRange => UnityEngine.Random.Range(min, max);

    public FloatRange(float value) { min = max = value; }

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max < min ? min : max;
    }
}

public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }
    public FloatRangeSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }
}

public class HistoryQueue<T>
{
    public int Count => queue.Count;

    private int capacity;
    private Queue<T> queue;

    private T[] array = null;

    public HistoryQueue(int capacity)
    {
        this.capacity = capacity;
        queue = new Queue<T>();
    }

    public void Enqueue(T item)
    {
        while (queue.Count >= capacity)
        {
            queue.Dequeue();
        }

        queue.Enqueue(item);
        array = queue.ToArray();
    }

    public void Clear()
    {
        queue.Clear();
        array = queue.ToArray();
    }

    public T At(int i)
    {
        return array[i];
    }
}

public class Extra
{
    public static Vector3 ClampMagnitude(Vector3 vector, float max, float min, bool isFloatSquared)
    {
        double squareMagnitude = vector.sqrMagnitude;
        float squaredMax = max, squaredMin = min;
        if (!isFloatSquared)
        {
            squaredMax *= max;
            squaredMin *= min;
        }
        if (squareMagnitude > squaredMin) return vector.normalized * squaredMax;
        else if (squareMagnitude < squaredMin) return vector.normalized * squaredMin;
        return vector;
    }

    public static IEnumerator LerpTranslate(Transform transform, Vector3 targetPoint, float speed)
    {
        float progress = 0f;
        while (progress <= 1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPoint, speed * Time.deltaTime);
            progress += speed * Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator LerpLookRotate(Transform transform, Vector3 targetPoint, float speed)
    {
        float turnProgress = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        while (turnProgress < 1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
            turnProgress += speed * Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator LerpLookAt(Transform transform, Vector3 targetPoint, float speed)
    {
        float turnProgress = 0f;
        while (turnProgress < 1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
            turnProgress += speed * Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator FadeSwitch(Image[] toFade, Image[] toShow, float speed, Action callback = null)
    {
        Color imageColor;

        float progress = 0f;
        while (progress < 1f)
        {
            foreach (Image image in toFade)
            {
                imageColor = image.color;
                imageColor.a = Mathf.Max(0f, 1 - progress);
                image.color = imageColor;
            }

            foreach (Image image in toShow)
            {
                imageColor = image.color;
                imageColor.a = Mathf.Min(1f, progress);
                image.color = imageColor;
            }

            progress += speed;
            yield return null;
        }

        foreach (Image image in toShow)
        {
            imageColor = image.color;
            imageColor.a = 1f;
            image.color = imageColor;
        }

        callback?.Invoke();
    }

    public static string MoneyFormat(float value)
    {
        return value >= 1000f ? (value / 1000f).ToString("0.0") + "K" : ((int)value).ToString();
    }
}
