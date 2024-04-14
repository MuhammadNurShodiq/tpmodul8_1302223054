using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidInput covidInput = new CovidInput();

        Console.WriteLine("Berapa suhu badan anda saat ini?");
        suhuBadan = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam?");
        hariDemam = Convert.ToInt32(Console.ReadLine());

        bool tepatWaktu = hariDemam < covidInput.covidConfig.batas_hari_demam;
        bool terimaFahrenheit = (covidInput.covidConfig.satuan_suhu == "fahrenheit") && (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (covidInput.covidConfig.satuan_suhu == "celcius") && (suhuBadan >= 36.5 && suhuBadan <= 37.5);

        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(covidInput.covidConfig.pesan_diterima);
        }

        else
        {
            Console.WriteLine(covidInput.covidConfig.pesan_ditolak);
        }
    }

    public class CovidConfig
    {
        public String satuan_suhu { get; set; }
        public int batas_hari_demam { get; set; }
        public String pesan_ditolak { get; set; }
        public String pesan_diterima { get; set; }

        public CovidConfig() { }
        public CovidConfig(string satuan_suhu, int batas_hari_demam, string pesan_ditolak, string pesan_diterima)
        {
            this.satuan_suhu = satuan_suhu;
            this.batas_hari_demam = batas_hari_demam;
            this.pesan_ditolak = pesan_ditolak;
            this.pesan_diterima = pesan_diterima;
        }
    }

    public class CovidInput
    {
        public CovidConfig covidConfig;
        public const String filepath = @"covidconfig.json";

        public CovidInput() {
            try
            {
                ReadConfig();
            }

            catch (Exception)
            {
                SetDefault();
                WriteNewConfig();
            }
        }

        private CovidConfig ReadConfig()
        {
            string jsonData = File.ReadAllText(filepath);
            covidConfig = JsonSerializer.Deserialize<CovidConfig>(jsonData);
            return covidConfig;
        }

        private void SetDefault()
        {
            covidConfig = new CovidConfig("celcius", 14, "Anda tidak diperbolehkan masuk ke dalam gedung ini", "Anda dipersilahkan untuk masuk ke dalam gedung ini");
        }

        private void WriteNewConfig()
        {
            JsonSerializerOptions options = new JsonSerializerOptions() 
            {
                WriteIndented = true,
            };

            string JsonString = JsonSerializer.Serialize(covidConfig, options);
            File.WriteAllText(filepath, JsonString);
        }

        public void UbahSatuan (string satuanBaru)
        {
            bool satuanValid = (satuanBaru == "celcius" || satuanBaru == "fahrenheit");

            if (satuanBaru == null || !satuanValid)
            {
                throw new ArgumentException();
            }

            if (satuanValid)
            {
                covidConfig.satuan_suhu = satuanBaru;

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };

                string updateSuhu = JsonSerializer.Serialize(covidConfig, jsonOptions);
                File.WriteAllText (filepath, updateSuhu);
            }
        }
    }
}