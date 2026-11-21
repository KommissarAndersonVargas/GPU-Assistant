using GpuAssistant.Models;

namespace GpuAssistant.BLL
{
    public static class GpuBLL
    {
        private const string GpuBrand = "NVIDIA"; // alterar caso mude a marca

        // dados simulados (fonte da verdade) para evitar banco de dados ou arquivos
        private static readonly List<Gpu> Gpus = new()
        {
            new Gpu { Name = "NVIDIA A100",     Quantity = 4, MemoryGB = 80, InUse = 1 },
            new Gpu { Name = "NVIDIA L40S",     Quantity = 6, MemoryGB = 48, InUse = 2 },
            new Gpu { Name = "NVIDIA RTX 4090", Quantity = 3, MemoryGB = 24, InUse = 3 },
            new Gpu { Name = "NVIDIA H100",     Quantity = 2, MemoryGB = 80, InUse = 0 },
            new Gpu { Name = "NVIDIA A40",      Quantity = 4, MemoryGB = 48, InUse = 1 },
        };

        // lista completa com resumo já calculado (enviada no system prompt)
        public static string ListGpus()
        {
            var lines = Gpus.Select(g =>
                $"- {g.Name}: total {g.Quantity}, disponíveis {g.Available}, em uso {g.InUse}, memória {g.MemoryGB} GB");

            var summary = $"Resumo: {Gpus.Count} modelos de GPU, {Gpus.Sum(g => g.Quantity)} GPUs no total, " +
                          $"{Gpus.Sum(g => g.Available)} disponíveis.";

            return summary + "\n" + string.Join("\n", lines);
        }

        // procura GPUs citadas na pergunta e devolve os dados delas (anexados à mensagem do usuário)
        public static string? FindRelevantData(string userInput)
        {
            var text = Normalize(userInput);

            var found = Gpus.Where(g =>
            {
                var fullName = Normalize(g.Name);                    // ex: "RTX4090"
                var firstPart = Normalize(g.Name.Split(' ')[1]);     // ex: "RTX"
                return text.Contains(fullName) || text.Contains(firstPart);
            }).ToList();

            return found.Count > 0
                ? string.Join("\n", found.Select(g => $"{g.Name}: {g.Available} disponíveis de {g.Quantity}, memória {g.MemoryGB} GB"))
                : null;
        }

        // remove marca e espaços: "NVIDIA RTX 4090" → "RTX4090"
        private static string Normalize(string value)
        {
            return value.ToUpperInvariant().Replace(GpuBrand, "").Replace(" ", "");
        }
    }
}