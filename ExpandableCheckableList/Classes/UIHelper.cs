using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;

using System;

namespace ExpandableCheckableList
{
    public static class UIHelper
    {
        static readonly string TAG = nameof(UIHelper);

        public static int GetThemeTextPrimaryColor(Context context)
        {
            int primaryColor = -1;

            TypedArray arr = null;
            try
            {
                // Get the primary text color of the theme
                TypedValue typedValue = new TypedValue();
                context.Theme.ResolveAttribute(Android.Resource.Attribute.TextColorPrimary, typedValue, true);
                arr = context.ObtainStyledAttributes(typedValue.Data, new int[] { Android.Resource.Attribute.TextColorPrimary });
                primaryColor = arr.GetColor(0, -1);
            }
            catch { }
            finally
            {
                arr.Recycle();
            }

            return primaryColor;
        }

        public static int GetThemeTextPrimaryInverseColor(Context context)
        {
            int color = -1;

            TypedArray arr = null;
            try
            {
                // Get the primary text color of the theme
                TypedValue typedValue = new TypedValue();
                context.Theme.ResolveAttribute(Android.Resource.Attribute.TextColorPrimaryInverse, typedValue, true);
                arr = context.ObtainStyledAttributes(typedValue.Data, new int[] { Android.Resource.Attribute.TextColorPrimaryInverse });
                color = arr.GetColor(0, -1);
            }
            catch (Exception e)
            {
                Log.Error(TAG + $".{nameof(GetThemeTextPrimaryInverseColor)}", e.Message);
            }
            finally
            {
                arr.Recycle();
            }

            return color;
        }

        public static int ResolveColorAttr(Context context, int colorAttr)
        {
            TypedValue resolvedAttr = ResolveThemeAttr(context, colorAttr);
            // resourceId is used if it's a ColorStateList, and data if it's a color reference or a hex color
            int colorRes = resolvedAttr.ResourceId != 0 ? resolvedAttr.ResourceId : resolvedAttr.Data;
            //var colorList = ContextCompat.GetColorStateList(context, colorRes);
            return ContextCompat.GetColor(context, colorRes);
        }

        public static TypedValue ResolveThemeAttr(Context context, int attrRes)
        {
            var theme = context.Theme;
            TypedValue typedValue = new TypedValue();
            theme.ResolveAttribute(attrRes, typedValue, true);
            return typedValue;
        }

        public static int ResolveDimensionAttr(Context context, int attrId)
        {
            TypedValue typedValue = ResolveThemeAttr(context, attrId);
            int[] attrs = new int[] { attrId };
            int indexOfAttrTextSize = 0;
            TypedArray a = context.ObtainStyledAttributes(typedValue.Data, attrs);
            int value = a.GetDimensionPixelSize(indexOfAttrTextSize, -1);
            a.Recycle();

            return value;
        }

        public static Drawable ResolveDrawableAttr(Context context, int attrId)
        {
            TypedValue typedValue = ResolveThemeAttr(context, attrId);
            int[] attrs = new int[] { attrId };
            //int indexOfAttrTextSize = 0;
            TypedArray a = context.ObtainStyledAttributes(typedValue.Data, attrs);
            var value = a.GetDrawable(0);
            a.Recycle();

            return value;
        }
    }
}