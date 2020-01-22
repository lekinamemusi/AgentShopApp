using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace AgentShopApp.Droid.Permissions
{
    public static class GenericPermissionRequester
    {
        public static void RequestAllRequiredPermission(Context context, string[] permissionList)
        {
            foreach (var permission in permissionList)
                RequestRequiredPermission(context, permission);
        }

        public static void RequestRequiredPermission(Context context, string permission)
        {
            if (ContextCompat.CheckSelfPermission(context, permission) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
            }
            else
            {
                // Camera permission is not granted. If necessary display rationale & request.
            }
        }

        ////public static void RequestPermissionWithRationale(Context context,string permission)
        ////{
        ////    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
        ////    {
        ////        // Provide an additional rationale to the user if the permission was not granted
        ////        // and the user would benefit from additional context for the use of the permission.
        ////        // For example if the user has previously denied the permission.
        ////        Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

        ////        var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
        ////        Snackbar.Make(layout,
        ////                       Resource.String.permission_location_rationale,
        ////                       Snackbar.LengthIndefinite)
        ////                .SetAction(Resource.String.ok,
        ////                           new Action<View>(delegate (View obj) {
        ////                               ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
        ////                           }
        ////                )
        ////        ).Show();
        ////    }
        ////    else
        ////    {
        ////        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_LOCATION);
        ////    }
        ////}
    }
}