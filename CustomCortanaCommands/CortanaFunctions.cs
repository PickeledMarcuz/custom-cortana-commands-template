using System;
using System.Collections.Generic;
using Windows.UI.Popups;

using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;

using Windows.System;
using Windows.Media.SpeechRecognition;
using Windows.ApplicationModel.Activation;


/*crclayton was a great assist to the ideas behind this template */

namespace CustomCortanaCommands
{

    class CortanaFunctions
    {
        /*
        This is the lookup of VCD CommandNames as defined in 
        CustomVoiceCommandDefinitios.xml to their corresponding actions
        */
        private readonly static IReadOnlyDictionary<string, Delegate> vcdLookup = new Dictionary<string, Delegate>{

            /*
            {<command name from VCD>, (Action)(async () => {
                 <code that runs when that commmand is called>
            })}
            */

            {"OpenWebsite", (Action)(async () => {
                 Uri website = new Uri(@"https://vrvoicedemo.sharepoint.com");
                 await Launcher.LaunchUriAsync(website);
             })},

            {"ListFiles", (Action)(async () =>
            {
                Uri website = new Uri(@"https://vrvoicedemo.sharepoint.com/Lists");
                await Launcher.LaunchUriAsync(website);
            })},

            {"OpenFile", (Action)(async () => {
                StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"Test.txt");
                await Launcher.LaunchFileAsync(file);
            })},

            {"CreateFile", (Action)(async () => {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.CreateFileAsync(
                    @"NewFile.txt", CreationCollisionOption.ReplaceExisting);

                await storageFolder.GetFileAsync("NewFile.txt");
                await FileIO.WriteTextAsync(sampleFile, "This file was created by Cortana at " + DateTime.Now);
            })},

        };

        /*
        Register Custom Cortana Commands from VCD file
        */
        public static async void RegisterVCD()
        {
            StorageFile vcd = await Package.Current.InstalledLocation.GetFileAsync(
                @"CustomVoiceCommandDefinitions.xml");

            await VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(vcd);
        }

        /*
        Look up the spoken command and execute its corresponding action
        */
        public static void RunCommand(VoiceCommandActivatedEventArgs cmd)
        {
            SpeechRecognitionResult result = cmd.Result;
            string commandName = result.RulePath[0];
            vcdLookup[commandName].DynamicInvoke();
        }
    }
}
