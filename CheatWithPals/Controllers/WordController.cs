using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CheatWithPals.Controllers
{
    public class WordController : ApiController
    {
        private EnglishWordsEntities db = new EnglishWordsEntities();

        // GET api/Word
        public IEnumerable<Word> GetWords()
        {
            return db.Words.AsEnumerable();
        }

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