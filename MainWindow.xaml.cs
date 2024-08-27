using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace MechEngineerSaver;

public partial class MainWindow : Window
{
    public MainWindowContext Context =>
        (MainWindowContext)((FrameworkElement)Content).DataContext;

    public MainWindow()
    {
        this.InitializeComponent();

        Thumb.DragDelta += (s, e) =>
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        };
    }


    private void Click_Exit(object sender, RoutedEventArgs e) => App.Current.Shutdown();
}

public class MainWindowContextHolder : ContextHolder<MainWindowContext> { }
public partial class MainWindowContext : ObservableObject
{
    public ProgressManager ProgressManager => ProgressManager.Instance;

    FileInfo SavesInfo =
        new($@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Mech_Engineer\data\{nameof(MechEngineerSaver)}\Info.json");
    FileInfo CurrentSaveFile =
        new($@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Mech_Engineer\data\f_save_slot_1");

    public MainWindowContext()
    {
        SaverInfoInitialize();
        UpdateInfo();
        UpdatesScaner();
        LastWriteUpdate();
    }

    public void SaverInfoInitialize()
    {
        if (!SaveInfo.SavesDirectoryOwn.Exists)
            SaveInfo.SavesDirectoryOwn.Create();

        if (!SavesInfo.Exists)
        {
            using var sw = SavesInfo.CreateText();
            sw.WriteLine(JsonSerializer.Serialize(Saves));
        }
        else
            Saves.Reset(JsonSerializer.Deserialize<ObservableCollection<SaveInfo>>(SavesInfo.ReadAllText()));
    }


    [ObservableProperty] int _Day;
    [ObservableProperty] TimeSpan _Battletime;
    [ObservableProperty] int _Scientists;
    [ObservableProperty] int _Engineers;
    [ObservableProperty] int _Skalaknit;
    [ObservableProperty] int _Munilon;
    [ObservableProperty] int _Bjorn;
    [ObservableProperty] int _Metallite;

    partial void OnSelectedSaveChanged(SaveInfo value) => UpdateDelta();
    void UpdateDelta()
    {
        if (SelectedSave != null)
        {
            D_Skalaknit = Skalaknit - SelectedSave.Skalaknit;
            D_Munilon = Munilon - SelectedSave.Munilon;
            D_Bjorn = Bjorn - SelectedSave.Bjorn;
            D_Metallite = Metallite - SelectedSave.Metallite;
        }
    }
    [ObservableProperty] int _D_Skalaknit;
    [ObservableProperty] int _D_Munilon;
    [ObservableProperty] int _D_Bjorn;
    [ObservableProperty] int _D_Metallite;


    readonly static Regex reg = new("^.*=\"(\\d+).000000\"$", RegexOptions.IgnoreCase);
    public async void UpdateInfo(bool delay = false)
    {
        if (File.Exists(CurrentSaveFile.FullName))
        {
            if (delay)
                await Task.Delay(100).ConfigureAwait(true);

            var saveData = CurrentSaveFile.ReadAllText().Split("ammo_red")[1].Split("\r\n");

            Battletime = new(0, 0, getValue(saveData, "battletime="));
            
            Day = getValue(saveData, "ingame_day=");
            Scientists = getValue(saveData, "res_science_team=");
            Engineers = getValue(saveData, "res_staff=");
            Skalaknit = getValue(saveData, "res_skalaknit=");
            Munilon = getValue(saveData, "res_munilon=");
            Bjorn = getValue(saveData, "res_bjorn=");
            Metallite = getValue(saveData, "res_metallite=");

            UpdateDelta();
        }
    }
    static int getValue(IEnumerable<string>? data, string parameter)
    {
        if (data != null)
        {
            var rawValue = data.FirstOrDefault(s => s.Contains(parameter));
            if (rawValue != null)
            {
                var match = reg.Match(rawValue);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int result))
                    return result;
            }
        }
        return 0;
    }



    public DateTime? LastWrite;
    public Task UpdatesScaner() => Task.Run(async () =>
    {
        DateTime dt;
        while (true)
        {
            await Task.Delay(500);

            DateTime newLWT = new(); var TimeNow = DateTime.Now;
            foreach (var file in SaveInfo.SaveFiles)
            {
                dt = File.GetLastWriteTime(file.FullName);
                if (dt > newLWT)
                    newLWT = dt;
            }
            if (LastWrite != newLWT)
                UpdateInfo(true);
            LastWrite = newLWT;
        }
    });

    [ObservableProperty] string _LastWriteString;
    [ObservableProperty] Visibility _Visability_AnimatedText = Visibility.Collapsed;
    private static TimeSpan t_10Minut = new(0, 10, 0);
    private static TimeSpan t_Minute = new(0, 1, 0);
    private static TimeSpan t_Hour = new(1, 0, 0);
    private static TimeSpan t_Day = new(1, 0, 0, 0);
    private static TimeSpan t_2Day = new(1, 0, 0, 0);
    private DateTime lastLT = DateTime.MinValue;
    public Task LastWriteUpdate() => Task.Run(async () =>
    {
        while (true)
        {
            await Task.Delay(1000);
            var t = DateTime.Now;
            string nlwt; TimeSpan delta; DateTime lw; bool nlwtb;
            if (LastWrite != null)
            {
                lw = LastWrite.Value;
                delta = t - lw;

                {
                    nlwtb = lastLT != lw;
                    lastLT = lw;
                    if (nlwtb)
                    {
                        Visability_AnimatedText = Visibility.Visible;
                    }
                }

                if (delta < t_Minute) LastWriteString = $"{delta.Seconds} sec";
                else if (delta < t_10Minut) LastWriteString = $"{delta.Minutes} min {delta.Seconds} sec";
                else if (delta < t_Hour)
                {
                    nlwt = $"{delta.Minutes} min";
                    if (LastWriteString != nlwt) LastWriteString = nlwt;
                }
                else if (delta < t_Day)
                {
                    nlwt = $"{delta.Hours} hours";
                    if (LastWriteString != nlwt) LastWriteString = nlwt;
                }
                else if (delta < t_2Day)
                {
                    nlwt = $"day {delta.Hours} h";
                    if (LastWriteString != nlwt) LastWriteString = nlwt;
                }
                else if (delta >= t_2Day)
                {
                    nlwt = $"{delta.Days} d {delta.Hours} h";
                    if (LastWriteString != nlwt) LastWriteString = nlwt;
                }

                {
                    if (nlwtb)
                    {
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(6000);
                            Visability_AnimatedText = Visibility.Collapsed;
                        });
                    }
                }

                continue;
            }
            LastWriteString = "Last Write";
        }
    });




    public DispatchedCollection<SaveInfo> Saves { get; } = [];
    [ObservableProperty] SaveInfo _SelectedSave;
    [ObservableProperty] int _SelectedIndex;
    [ObservableProperty] string _NewSaveName;

    [RelayCommand] Task CreateSave() => Task.Run(() =>
    {
        var newSave = SaveInfo.CreateNew(this);
        Saves.Insert(0, newSave);
        SelectedSave = newSave;
        SavesInfo.WriteAllText(JsonSerializer.Serialize(Saves));
    });

    [RelayCommand] Task RestoreSave() => SelectedSave.LoadSave();

    [RelayCommand] Task DeleteSave() => Task.Run(() =>
    {
        var ss = SelectedSave;
        SelectedSave = null!;
        ss.DeleteSave();
        var lastIndex = SelectedIndex;
        Saves.Remove(ss);
        {
            for (int i = 0; i < Saves.Count; i++)
                Saves[i].OrderNumber = Saves.Count - i;

            if (Saves.Count > lastIndex + 1)
                SelectedIndex = lastIndex;
            else if (Saves.Count > 0)
                SelectedSave = Saves.Last();
        }
        SavesInfo.WriteAllText(JsonSerializer.Serialize(Saves));
        
    });
}

public partial class ProgressManager : ObservableObject
{
    private ProgressManager() { }
    public static ProgressManager Instance => instance ??= new ProgressManager();
    static ProgressManager? instance;


    private readonly SolidColorBrush FoneBrush = new(Color.FromArgb(255, 30, 30, 30));
    [ObservableProperty] SolidColorBrush _ProgressBackground = Brushes.Transparent;
    [ObservableProperty] double _ProgressWidth;
    public void ProgressSet(double coeff, bool begin = false)
    {
        if (begin) ProgressBackground = FoneBrush;
        ProgressWidth = 300 * coeff;
    }
    public void ProgressAdd(double increment)
    {
        ProgressWidth += 300 * increment;
    }
    public async void ProgressEnd()
    {
        ProgressWidth = 300;
        await Task.Delay(200);
        ProgressBackground = Brushes.Transparent;
        ProgressWidth = 0;
    }
}

public partial class SaveInfo : ObservableObject, IOrderedElement
{
    static ProgressManager ProgressManager => ProgressManager.Instance;

    public static DirectoryInfo SavesDirectoryBase =
        new($@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Mech_Engineer\data");

    public static DirectoryInfo SavesDirectoryOwn =
        new($@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Mech_Engineer\data\{nameof(MechEngineerSaver)}");

    public static readonly ReadOnlyCollection<string> IgnoreFiles = new(["settings.ini"]);
    public static IEnumerable<FileInfo> SaveFiles => SavesDirectoryBase.GetFiles().Where(f => !IgnoreFiles.Contains(f.Name));

    public static SaveInfo CreateNew(MainWindowContext CD)
    {
        var SI = new SaveInfo()
        {
            SaveName = CD.NewSaveName,
            LastWrite = CD.LastWrite!.Value,
            TimeStamp = DateTime.Now,
            OrderNumber = CD.Saves.Count + 1,

            Day = CD.Day,
            Battletime = CD.Battletime,
            Scientists = CD.Scientists,
            Engineers = CD.Engineers,
            Skalaknit = CD.Skalaknit,
            Munilon = CD.Munilon,
            Bjorn = CD.Bjorn,
            Metallite = CD.Metallite,
        };
        SI.CreateArchive().Wait();
        return SI;
    }

    [ObservableProperty] int _OrderNumber;
    [ObservableProperty] string _SaveName = null!;
    [ObservableProperty] DateTime _LastWrite;
    [ObservableProperty] DateTime _TimeStamp;


    [ObservableProperty] int _Day;
    [ObservableProperty] TimeSpan _Battletime;
    [ObservableProperty] int _Scientists;
    [ObservableProperty] int _Engineers;
    [ObservableProperty] int _Skalaknit;
    [ObservableProperty] int _Munilon;
    [ObservableProperty] int _Bjorn;
    [ObservableProperty] int _Metallite;



    [property: JsonIgnore] public string SaveFilePath => $@"{SavesDirectoryOwn}\save_{TimeStamp.Ticks}";


    Task CreateArchive() => Task.Run(() =>
    {
        if (!SavesDirectoryOwn.Exists)
            SavesDirectoryOwn.Create();
        using FileStream zipToOpen = new(SaveFilePath, FileMode.Create);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);

        foreach (var file in SaveFiles)
        {
            using var fileStream = file.OpenRead();
            using var archiveStream = archive.CreateEntry(file.Name).Open();
            fileStream.CopyTo(archiveStream);
        }
    });

    public Task LoadSave() => Task.Run(() =>
    {
        ProgressManager.ProgressSet(0, true);
        ProgressManager.ProgressSet(0.25);
        foreach (var f in SaveFiles)
        {
            Task.Run(() =>
            {
                while (File.Exists(f.FullName))
                { try { File.Delete(f.FullName); } catch { Task.Delay(100).Wait(); } }
            }).
            Wait();
        }
        ProgressManager.ProgressSet(0.5);
        ZipFile.ExtractToDirectory(SaveFilePath, SavesDirectoryBase.FullName, true);
        ProgressManager.ProgressSet(0.75);
        ProgressManager.ProgressEnd();
    });

    public Task DeleteSave() => Task.Run(() =>
    {
        while (File.Exists(SaveFilePath))
        { try { File.Delete(SaveFilePath); } catch { Task.Delay(100).Wait(); } }
    });
}