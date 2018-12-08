using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace FileAccessSample
{
    public partial class MainPage : ContentPage
    {
        // global list to store the data
        // for CRUD
        List<Dogs> dogs;
        // == global constants ==
        private const string OUTPUT_FILE = "jsonDogs.txt";

        public MainPage()
        {
            InitializeComponent();
            if (dogs == null)
            {
                dogs = new List<Dogs>();
            }
            SetUpLists();
            // set the dogs list as the source for the list view.
            lvDogs.ItemsSource = dogs;
        }

        private void SetUpLists()
        {
            // if (dogs != null) return;
            //list is not created, instantiate and fill
            // dogs = new List<Dogs>();

            //try to read the local File
            //if that doesnt work, then read the dll
            try
            {
                #region Read file from Local Application Data
                //get the local application directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                //add a file name to the path
                string FileName = Path.Combine(path, OUTPUT_FILE);
                //create a streamwriter to write to the file
                using (var streamReader = new StreamReader(FileName, false))
                {
                    string jsonString = streamReader.ReadToEnd();
                    dogs = JsonConvert.DeserializeObject<List<Dogs>>(jsonString);
                   
                    
                }
                #endregion
            }
            catch (Exception)
            {





                #region READ FILE FROM DLL

                // need a link to the assembly (dll) to get the file
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
                // create a stream to access the file
                Stream stream = assembly.GetManifestResourceStream(
                                    "FileAccessSample.Data.DogsJson.txt");
                // create a stream reader to read from the stream
                using (var reader = new StreamReader(stream))
                {
                    string jsonText = reader.ReadToEnd();
                    // use a Json parser to deserialize the text in file
                    // to a list of object of type Dogs
                    dogs = JsonConvert.DeserializeObject<List<Dogs>>(jsonText);

                }
                #endregion
            }
        }

        private void lvDogs_ItemSelected(object sender, 
                                    SelectedItemChangedEventArgs e)
        {
            Dogs current = (Dogs)e.SelectedItem;

            //get the data from current into slOneDog
            //slOneDog has a BindingContext attribute
            s1OneDog.BindingContext = current;
        }

        private void btnUpdateOneDog_Clicked(object sender, EventArgs e)
        {
            //when the user changes text in the entry
            //update the entry in the list with data from slOneDog
            //find which entry in the list
            foreach (var dog in dogs)
            {
                if (dog.Breed == lblBreed.Text)
                {
                    //copy the data
                    dog.Category = entCategory.Text;
                    dog.Origin = entOrigin.Text;
                    dog.Exercise = entExercise.Text;
                    dog.Grooming = entGrooming.Text;
                }
            }
            RefreshList();
        }

        private void RefreshList()
        {
            lvDogs.ItemsSource = null;//empties the list
            lvDogs.ItemsSource = dogs;
        }

        private void DogPropertyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            //enable Button to allow user to save changes to the list
            btnUpdateOneDog.IsEnabled = true;
        }

        private void btnSaveToFile_Clicked(object sender, EventArgs e)
        {
            //save the list dogs to a file
            //serialise to a string using jsonConvert

            //get the local application directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //add a file name to the path
            string FileName = Path.Combine(path, OUTPUT_FILE);
            //create a streamwriter to write to the file
            using (var streamWriter = new StreamWriter(FileName,false))
            {
                string jsonString = JsonConvert.SerializeObject(dogs);
                streamWriter.WriteLine(jsonString);
            }
        }

        private void btnReadFromFile_Clicked(object sender, EventArgs e)
        {
            SetUpLists();
        }
    }
}
