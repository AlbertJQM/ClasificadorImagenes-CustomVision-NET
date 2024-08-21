using ClasificadorImagenes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClasificadorImagenes.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CustomVisionService _customVisionService;

        public IndexModel(CustomVisionService customVisionService)
        {
            _customVisionService = customVisionService;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IList<PredictionModel> Predictions { get; set; }
        public string ImageUrl { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload != null && Upload.Length > 0)
            {
                //using var stream = Upload.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await Upload.CopyToAsync(memoryStream);

                var base64Image = Convert.ToBase64String(memoryStream.ToArray());
                ImageUrl = $"data:{Upload.ContentType};base64,{base64Image}";

                memoryStream.Position = 0;
                Predictions = await _customVisionService.ClassifyImageFromImageAsync(memoryStream);
                return Page();
            }
            return BadRequest("No se recibió ninguna imagen.");
        }

        public void OnGet()
        {

        }
    }
}
