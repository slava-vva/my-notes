using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Configuration;

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

        public List<Note>? Notes { get; set; }

        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

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

        public List<Note> GetNotes()
        {
            return Notes = context.Notes.OrderByDescending(n => n.CreatedDate).Take(3).ToList();
        }
        public List<Note> SearchNotes(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return GetNotes();
            }

            List<Note> res = new List<Note>();

            string normalizedTerm = searchText.ToLowerInvariant();
            string[] searchTerms = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (searchTerms.Length > 0)
            {
                normalizedTerm = searchTerms[0].ToLowerInvariant();
                res = context.Notes.Where(n => (n.Name != null && n.Name.Contains(normalizedTerm))
                || (n.Description != null && n.Description.Contains(normalizedTerm))
                || (n.links != null && n.links.Contains(normalizedTerm))
                || (n.Tags != null && n.Tags.Contains(normalizedTerm))).ToList();
            }
            
            if (searchTerms.Length > 1 && res.Count > 0)
            {
                foreach (var term in searchTerms)
                {
                    if (string.IsNullOrEmpty(term)) continue;
                    // Normalize the term to lower case for case-insensitive search
                    normalizedTerm = term.ToLowerInvariant();
                    // Filter notes based on the normalized term
                    res = res.Where(n => (n.Name != null && n.Name.ToLowerInvariant().Contains(normalizedTerm))
                        || (n.Description != null && n.Description.ToLowerInvariant().Contains(normalizedTerm))
                        || (n.links != null && n.links.ToLowerInvariant().Contains(normalizedTerm))
                        || (n.Tags != null && n.Tags.ToLowerInvariant().Contains(normalizedTerm))
                        ).ToList();

                }
            }

            //res = context.Notes.Where(n => (n.Name != null && n.Name.Contains(searchText)) 
            //|| (n.Description != null && n.Description.Contains(searchText))
            //|| (n.Tags != null && n.Tags.Contains(searchText))
            //).ToList();

            TotalCount = res.Count;

            return Notes = res;
        }

    }
}
