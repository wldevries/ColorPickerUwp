using ColorPickerShared.DTO;
using ColorPickerShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ColorPickerShared.Services
{
    public class SessionSaver
    {
        public static async Task SaveAsync(IEnumerable<ColorGroupViewModel> groups)
        {
            var dtos = groups.Select(g => g.ToDTO()).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(dtos);
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "session.json",
                    CreationCollisionOption.ReplaceExisting);
                
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
            }
        }

        public static async Task<IReadOnlyCollection<ColorGroupViewModel>> LoadAsync()
        {
            try
            {
                var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("session.json");
                if (item is IStorageFile file)
                {
                    // Load json data from sessionFile
                    var json = await FileIO.ReadTextAsync(file);
                    var dtos = System.Text.Json.JsonSerializer.Deserialize<List<ColorGroupDTO>>(json);
                    return dtos.Select(dto => ColorGroupViewModel.FromDTO(dto)).ToList();
                }
            }
            catch (Exception)
            {
            }

            return Array.Empty<ColorGroupViewModel>();
        }
    }
}
