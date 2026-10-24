using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyNotes.Models
{
    public class NotesModel
    {
        public AppDbContext context { get; set; }
        public NotesModel(AppDbContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Note? InputNote { get; set; }

        public List<Note> Notes { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public int TotalCount { get; set; }

        /// <summary>Number of notes shown per "page" before the "Show more" button.</summary>
        public const int DefaultPageSize = 20;

        /// <summary>How many notes were skipped before the current page.</summary>
        public int Skip { get; set; }

        public bool HasMore => Skip + Notes.Count < TotalCount;
        public int Remaining => Math.Max(0, TotalCount - Skip - Notes.Count);

        public void AddNote(Note note)
        {
            context.Notes.Add(note);
            context.SaveChanges();
        }

        public void UpdateNote(Note note)
        {
            context.Notes.Update(note);
            context.SaveChanges();
        }

        public List<Note> GetNotes(int skip = 0)
        {
            Skip = Math.Max(0, skip);
            TotalCount = context.Notes.Count();
            Notes = context.Notes
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .ThenByDescending(n => n.Id)
                .Skip(Skip)
                .Take(PageSize)
                .ToList();
            return Notes;
        }

        /// <summary>
        /// Every word must appear (case-insensitive) in the topic, description, links or tags.
        /// Filtering is done in memory so that case-insensitivity also works for non-Latin text
        /// (SQLite's LIKE/lower() only fold ASCII letters). Fine for a personal notes database.
        /// </summary>
        public List<Note> SearchNotes(string? searchText, int skip = 0)
        {
            SearchTerm = searchText;
            var terms = SplitTerms(searchText);
            if (terms.Length == 0)
            {
                return GetNotes(skip);
            }

            var matches = context.Notes
                .AsNoTracking()
                .AsEnumerable()
                .Where(n => terms.All(t => Matches(n, t)))
                .OrderByDescending(n => n.UpdatedDate)
                .ThenByDescending(n => n.Id)
                .ToList();

            Skip = Math.Max(0, skip);
            TotalCount = matches.Count;
            Notes = matches.Skip(Skip).Take(PageSize).ToList();
            return Notes;
        }

        public static string[] SplitTerms(string? searchText) =>
            string.IsNullOrWhiteSpace(searchText)
                ? Array.Empty<string>()
                : searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static bool Matches(Note n, string term) =>
            Contains(n.Name, term) || Contains(n.Description, term) ||
            Contains(n.links, term) || Contains(n.Tags, term);

        private static bool Contains(string? field, string term) =>
            field != null && field.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}
