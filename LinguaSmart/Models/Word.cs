using System.ComponentModel.DataAnnotations;

namespace LinguaSmart.Models
{
    public class Word
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Въведете английска дума.")]
        [StringLength(100)]
        [Display(Name = "Английска дума")]
        public string EnglishWord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Въведете превод.")]
        [StringLength(100)]
        [Display(Name = "Превод на български")]
        public string BulgarianTranslation { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Значение")]
        public string? Definition { get; set; }

        [StringLength(500)]
        [Display(Name = "Примерно изречение")]
        public string? ExampleSentence { get; set; }


        [StringLength(500)]
        [Display(Name = "Синоними")]
        public string? Synonyms { get; set; }


        [Required(ErrorMessage = "Изберете категория.")]
        [Display(Name = "Категория")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Ниво на трудност")]
        public string DifficultyLevel { get; set; } = "Beginner";

        

        [Display(Name = "Дата на добавяне")]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
