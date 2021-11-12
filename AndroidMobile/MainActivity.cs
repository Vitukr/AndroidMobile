using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Xamarin.Essentials;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AndroidX.RecyclerView.Widget;

namespace AndroidMobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private readonly string _fileName = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), 
            "ServiceData.txt");
        private List<PassData> _data = new List<PassData>();
        private int numberFound = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
            buttonSave.Click += ButtonSaveOnClick;

            Button buttonFind = FindViewById<Button>(Resource.Id.buttonFind);
            buttonFind.Click += ButtonFind_Click;

            if (File.Exists(_fileName))
            {
                string records = File.ReadAllText(_fileName);
                _data = JsonConvert.DeserializeObject<PassData[]>(records).ToList();
            }
        }

        protected override void OnDestroy()
        {
            var jsonText = JsonConvert.SerializeObject(_data);
            File.WriteAllText(_fileName, jsonText);

            base.OnDestroy();
        }

        private void ButtonFind_Click(object sender, EventArgs e)
        {
            var data = _data.Select(r => r).ToList();
            EditText editTextResource = FindViewById<EditText>(Resource.Id.editTextResource);
            if(!string.IsNullOrEmpty(editTextResource.Text))
            {
                data = data.Where(r => r.Resource.Contains(editTextResource.Text)).ToList();
            }

            EditText editTextLogin = FindViewById<EditText>(Resource.Id.editTextLogin);
            if (!string.IsNullOrEmpty(editTextLogin.Text))
            {
                data = data.Where(r => r.Login.Contains(editTextLogin.Text)).ToList();
            }

            EditText editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            if (!string.IsNullOrEmpty(editTextPassword.Text))
            {
                data = data.Where(r => r.Password.Contains(editTextPassword.Text)).ToList();
            }
            if(string.IsNullOrEmpty(editTextResource.Text) && string.IsNullOrEmpty(editTextLogin.Text) && string.IsNullOrEmpty(editTextPassword.Text))
            {
                data = new List<PassData>();
            }

            TableLayout tableLayout = FindViewById<TableLayout>(Resource.Id.tableLayoutMain); // Resource.Layout
            TableLayout tableLayoutList = FindViewById<TableLayout>(Resource.Id.tableLayoutList);
            tableLayoutList.RemoveAllViews();

            //for (int i = 0; i < numberFound; i++)
            //{
            //    TableRow tableRow = FindViewById<TableRow>(2000 + i);

            //    tableLayoutList.RemoveView(tableRow);
            //}

            //android: layout_width = "match_parent"
            //android: layout_height = "wrap_content"
            //android: layout_marginLeft = "@dimen/abc_text_size_body_1_material"

            numberFound = data.Count;

            for (int i = 0; i < numberFound; i++)
            {
                EditText textView = new EditText(this);
                textView.Text = data[i].Resource + " " + data[i].Password;
                textView.Id = 1000 + i;
                //textView.LayoutParameters = new ViewGroup.LayoutParams(
                //    ViewGroup.LayoutParams.MatchParent,
                //    ViewGroup.LayoutParams.MatchParent);

                TableRow tableRow = new TableRow(this);
                tableRow.Id = 2000 + i;
                //tableRow.LayoutParameters = new ViewGroup.LayoutParams(
                //    ViewGroup.LayoutParams.MatchParent,
                //    ViewGroup.LayoutParams.MatchParent);
                tableRow.Left = 10;
                tableRow.AddView(textView);

                tableLayoutList.AddView(tableRow, i);
            }
        }

        private void ButtonSaveOnClick(object sender, EventArgs e)
        {
            EditText editTextResource = FindViewById<EditText>(Resource.Id.editTextResource);
            EditText editTextLogin = FindViewById<EditText>(Resource.Id.editTextLogin);
            EditText editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);

            PassData passData = new PassData() { Login = editTextLogin.Text, Password = editTextPassword.Text, Resource = editTextResource.Text };

            _data.Add(passData);
            var dataJson = JsonConvert.SerializeObject(_data);

            File.WriteAllText(_fileName, dataJson);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}

    struct PassData
    {
        public string Resource { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
