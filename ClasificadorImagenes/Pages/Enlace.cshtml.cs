using ClasificadorImagenes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClasificadorImagenes.Pages
{
    public class EnlaceModel : PageModel
    {
        private readonly CustomVisionService _customVisionService;

        public EnlaceModel(CustomVisionService customVisionService)
        {
            _customVisionService = customVisionService;
        }

        [BindProperty]
        public string Url { get; set; }

        public IList<PredictionModel> Predictions { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                Predictions = await _customVisionService.ClassifyImageFromUrlAsync(Url);
                return Page();
            }
            return BadRequest("No se recibió ninguna imagen.");            
        }

        public void OnGet()
        {
        }
    }

}
