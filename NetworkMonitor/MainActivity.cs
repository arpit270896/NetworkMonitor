﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Util;
using System.IO;

namespace NetworkMonitor
{
	[Activity (Label = "NetworkMonitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private TableLayout appDataTable;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			appDataTable = FindViewById<TableLayout>(Resource.Id.appData);
			// Set headings of table
			setTableRow ();

			// Instantiating an object of class
			GetAppDetails getAppDetails = new GetAppDetails();

			const string dirpath = "/proc/uid_stat";
			// Getting all the subdirectories /proc/uid_stat/*
			string[] subDirectories = Directory.GetDirectories(dirpath,"*");
			foreach (string subDirectory in subDirectories) 
			{
				// Get uid from the name of each subdirectory
				string uidText = subDirectory.Split ('/')[3];
				int uid = Convert.ToInt32 (uidText);

				// Reading files which contain data in each subdirectory
				string downFile = subDirectory + "/tcp_rcv";
				string upFile = subDirectory + "/tcp_snd";

				string upDataText = File.ReadAllText (downFile);
				string downDataText = File.ReadAllText (upFile);
				string totalDataPerUidText = "";

				double upData = Convert.ToInt64 (upDataText);
				double downData = Convert.ToInt64 (downDataText);
				double totalDataPerUid = upData + downData;

				// Convert bytes to suitable units
				var unitConverter = new UnitConverter();
				upDataText = unitConverter.unitConversion (upData);
				downDataText = unitConverter.unitConversion (downData);
				totalDataPerUidText = unitConverter.unitConversion (totalDataPerUid);

				// Get appName and appIcon for each uid
				string appName = getAppDetails.getAppNameForUid (uid);
				Drawable appIcon = getIconForUid (uid);

				// Appending items to view
				setTableRow (appName, upDataText, downDataText, totalDataPerUidText, appIcon);

				Log.Debug (uidText, appName);
			}
		}

		public Drawable getIconForUid(int uid)
		{
			string[] packagesName = { };
			Drawable appIcon = Resources.GetDrawable(Resource.Drawable.Icon);
			if (uid != 1000 && uid != 2000) {
				try {
					packagesName = PackageManager.GetPackagesForUid (uid);
				} catch (Exception e) {
					Console.WriteLine ("{0} Exception caught", e);
				}
				if (packagesName.Length == 1) {
					try {
						appIcon = PackageManager.GetApplicationIcon (PackageManager.GetApplicationInfo (packagesName [0], 0));
					} catch (Exception e) {
						Console.WriteLine ("{0} Exception caught", e);
					}
				}
			}
			return appIcon;
		}

		public void setTableRow (string appName = "AppName", string upDataText = "Upload", string downDataText = "Download", string totalDataPerUidText = "Total", Drawable appIcon = null)
		{
			var tr = new TableRow(this) ;
			if (appIcon != null) {
				ImageView c0 = new ImageView (this);
				c0.SetPadding (7, 7, 7, 7);
				c0.SetImageDrawable (appIcon);
				tr.AddView (c0);
			} else {
				TextView c0 = new TextView (this);
				c0.SetPadding (7, 7, 7, 7);
				c0.SetText ("AppIcon", TextView.BufferType.Editable);
				tr.AddView (c0);
			}
			TextView c1 = new TextView (this);
			c1.SetPadding (7, 7, 7, 7);
			if (appName != "AppName") {
				c1.TextSize = 20;
				c1.SetWidth (225);
			}
			c1.SetText (appName, TextView.BufferType.Editable);
			TextView c2 = new TextView (this);
			c2.SetPadding (7, 7, 7, 7);
			c2.SetText (upDataText, TextView.BufferType.Editable);
			TextView c3 = new TextView (this);
			c3.SetPadding (7, 7, 7, 7);
			c3.SetText (downDataText, TextView.BufferType.Editable);
			TextView c4 = new TextView (this);
			c4.SetPadding (7, 7, 7, 7);
			c4.SetText (totalDataPerUidText, TextView.BufferType.Editable);
			tr.AddView (c1);
			tr.AddView (c2);
			tr.AddView (c3);
			tr.AddView (c4);
			try {appDataTable.AddView (tr);}
			catch  (Exception e) {Console.WriteLine ("{0} Exception caught", e);}
		}

	}
}