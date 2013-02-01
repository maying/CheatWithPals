using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace CheatWithPals.Controllers
{
    public class WordController : ApiController
    {
        private EnglishWordsEntities db = new EnglishWordsEntities();

        // GET api/Words
        public IQueryable<Word> GetWords(ODataQueryOptions options, string boardLetter, string letterOnHand)
        {

            var appliedQuery = ((IQueryable<Word>)options.ApplyTo(db.Words)).ToList(); // apply the query and call toList so that I can do more operation (i.e. assign Point to Word)

            var regex = new Regex(string.Format(@"[{0}]", letterOnHand));
            var rtnval = (from word in appliedQuery
                          where Valid(boardLetter, letterOnHand, word.Word1)
                          select new Word
                          {
                              Word1 = word.Word1,
                              Point = CalculatePoint(word.Word1)
                          }).OrderByDescending(word => word.Point).AsQueryable();
            return rtnval;
        }

        string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        bool Valid(string boardLetter, string letterOnHand, string possibleWord)
        {
            var result = possibleWord;
            foreach (char c in boardLetter.ToLower() + letterOnHand.ToLower())
            {
                result = ReplaceFirst(result, c.ToString(), string.Empty);
            }

            return result.Length == 0;
        }

        int CalculatePoint(string word)
        {
            int point = 0;
            foreach (char c in word)
            {
                point += points[c];
            }

            return point;
        }

        Dictionary<char, int> points = new Dictionary<char, int>()
        {
            { 'a', 1 },
            { 'b', 3 },
            { 'c', 1 },
            { 'd', 2 },
            { 'e', 1 },
            { 'f', 4 },
            { 'g', 2 },
            { 'h', 4 },
            { 'i', 1 },
            { 'j', 8 },
            { 'k', 5 },
            { 'l', 1 },
            { 'm', 3 },
            { 'n', 1 },
            { 'o', 1 },
            { 'p', 3 },
            { 'q', 10 },
            { 'r', 1 },
            { 's', 1 },
            { 't', 1 },
            { 'u', 1 },
            { 'v', 4 },
            { 'w', 4 },
            { 'x', 8 },
            { 'y', 4 },
            { 'z', 10 },
        };


        // GET api/Word/5
        public Word GetWord(int id)
        {
            Word word = db.Words.Find(id);
            if (word == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return word;
        }

        // PUT api/Word/5
        public HttpResponseMessage PutWord(int id, Word word)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != word.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(word).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Word
        public HttpResponseMessage PostWord(Word word)
        {
            if (ModelState.IsValid)
            {
                db.Words.Add(word);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, word);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = word.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Word/5
        public HttpResponseMessage DeleteWord(int id)
        {
            Word word = db.Words.Find(id);
            if (word == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Words.Remove(word);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, word);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}