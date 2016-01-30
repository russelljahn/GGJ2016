using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace FlexiTweening.Extensions
{
    public static class ValueExtensions
    {
        public static ITween<float> TweenVolume([NotNull] this AudioSource audioSource)
        {
            if (audioSource == null) throw new ArgumentNullException("audioSource");

            return FlexiTween.From(audioSource.volume)
                .OnUpdate(volume => audioSource.volume = volume);
        }

        public static ITween<Vector3> TweenWorldPosition([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException("transform");

            return FlexiTween.From(transform.position)
                .OnUpdate(position => transform.position = position);
        }

        public static ITween<Vector3> TweenLocalPosition([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException("transform");

            return FlexiTween.From(transform.localPosition)
                .OnUpdate(position => transform.localPosition = position);
        }

        public static ITween<Vector2> TweenAnchoredPosition([NotNull] this RectTransform rectTransform)
        {
            if (rectTransform == null) throw new ArgumentNullException("rectTransform");

            return FlexiTween.From(rectTransform.anchoredPosition)
                .OnUpdate(position => rectTransform.anchoredPosition = position);
        }

        public static ITween<Quaternion> TweenWorldRotation([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException("transform");

            return FlexiTween.From(transform.rotation)
                .OnUpdate(rotation => transform.rotation = rotation);
        }

        public static ITween<Quaternion> TweenLocalRotation([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException("transform");

            return FlexiTween.From(transform.localRotation)
                .OnUpdate(rotation => transform.localRotation = rotation);
        }

        public static ITween<Vector3> TweenLocalScale([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException("transform");

            return FlexiTween.From(transform.localScale)
                .OnUpdate(scale => transform.localScale = scale);
        }

        public static ITween<Vector2> TweenSize([NotNull] this RectTransform rectTransform)
        {
            if (rectTransform == null) throw new ArgumentNullException("rectTransform");

            return FlexiTween.From(rectTransform.sizeDelta)
                .OnUpdate(delta => rectTransform.sizeDelta = delta);
        }

        public static ITween<float> TweenMinHeight([NotNull] this LayoutElement layoutElement)
        {
            if (layoutElement == null) throw new ArgumentNullException("layoutElement");

            return FlexiTween.From(layoutElement.minHeight)
                .OnUpdate(height => layoutElement.minHeight = height);
        }

        public static ITween<Vector4> TweenOffsets([NotNull] this RectTransform rectTransform)
        {
            if (rectTransform == null) throw new ArgumentNullException("rectTransform");

            var startValue = new Vector4(
                rectTransform.offsetMin.x, rectTransform.offsetMin.y,
                rectTransform.offsetMax.x, rectTransform.offsetMax.y);

            return FlexiTween.From(startValue)
                .OnUpdate(vector =>
                {
                    rectTransform.offsetMin = new Vector2(vector.x, vector.y);
                    rectTransform.offsetMax = new Vector2(vector.z, vector.w);
                });
        }

        public static ITween<float> TweenSize([NotNull] this Text text)
        {
            if (text == null) throw new ArgumentNullException("text");

            return FlexiTween.From(text.fontSize)
                .OnUpdate(size => text.fontSize = Mathf.FloorToInt(size));
        }

        public static ITween<Color> TweenColor([NotNull] this Graphic graphic)
        {
            if (graphic == null) throw new ArgumentNullException("graphic");

            return FlexiTween.From(graphic.color)
                .OnUpdate(color => graphic.color = color);
        }

        public static ITween<Color> TweenColor([NotNull] this SpriteRenderer spriteRenderer)
        {
            if (spriteRenderer == null) throw new ArgumentNullException("spriteRenderer");

            return FlexiTween.From(spriteRenderer.color)
                .OnUpdate(color => spriteRenderer.color = color);
        }

        public static ITween<Color> TweenColor([NotNull] this IList<SpriteRenderer> spriteRenderers)
        {
            if (spriteRenderers == null) throw new ArgumentNullException("spriteRenderers");

            return new Tween<Color>(Color.Lerp, spriteRenderers.First().color)
                .OnUpdate(color =>
                {
                    foreach (var renderer in spriteRenderers)
                    {
                        renderer.color = color;
                    }
                });
        }

        public static ITween<float> TweenAlpha([NotNull] this Graphic graphic)
        {
            if (graphic == null) throw new ArgumentNullException("graphic");

            return FlexiTween.From(graphic.color.a)
                .OnUpdate(alpha =>
                {
                    var color = graphic.color;
                    graphic.color = new Color(color.r, color.g, color.b, alpha);
                }
            );
        }

        public static ITween<float> TweenAlpha([NotNull] this CanvasGroup canvasGroup)
        {
            if (canvasGroup == null) throw new ArgumentNullException("canvasGroup");

            return FlexiTween.From(canvasGroup.alpha)
                .OnUpdate(alpha => canvasGroup.alpha = alpha);
        }

        public static ITween<float> TweenFill([NotNull] this Image image)
        {
            if (image == null) throw new ArgumentNullException("image");

            return FlexiTween.From(image.fillAmount)
                .OnUpdate(amount => image.fillAmount = amount);
        }

        public static ITween<float> TweenAlpha([NotNull] this Material material)
        {
            if (material == null) throw new ArgumentNullException("material");

            return FlexiTween.From(material.color.a)
                .OnUpdate(
                    value =>
                    {
                        material.color = new Color(material.color.r, material.color.g, material.color.b, value);
                    });
        }

        public static ITween<Color> TweenMask([NotNull] this Graphic graphic)
        {
            if (graphic == null) throw new ArgumentNullException("graphic");

            return FlexiTween.From(graphic.color)
                .OnUpdate(color => graphic.color = color);
        }
    }
}
