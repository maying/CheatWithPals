using CheatWithPals.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace CheatWithPals.Controllers
{
    public class Bonus
    {
        public BonusType Type { get; set; }
        public int Position { get; set; }
    }

    public enum BonusType
    {
        DL,DW,TL,TW
    }


    public class WordController : ApiController
    {
        private EnglishWordsEntities db = new EnglishWordsEntities();

        // GET api/Words
        public IQueryable<Word> GetWords(ODataQueryOptions options, 
            string boardLetter, string letterOnHand, [FromUri] Bonus[] bonuses = null)
        {

            var appliedQuery = ((IQueryable<Word>)options.ApplyTo(db.Words)).ToList(); // apply the query and call toList so that I can do more operation (i.e. assign Point to Word)

            var regex = new Regex(string.Format(@"[{0}]", letterOnHand));
            var rtnval = (from word in appliedQuery
                          where Valid(boardLetter, letterOnHand, word.Word1)
                          select new Word
                          {
                              Word1 = word.Word1,
                              Point = CalculatePoint(word.Word1, bonuses),
                              Def = GetDef(word),
                          }).OrderByDescending(word => word.Point).Take(10).AsQueryable();
            return rtnval;
        }

        

        string GetDef(Word wordObj)
        {
            var word = wordObj.Word1;

            if (!string.IsNullOrEmpty(wordObj.Def))
            {
                return wordObj.Def;
            }

            var httpClient = new HttpClient();
            var url = @"http://www.dictionaryapi.com/api/v1/references/collegiate/xml/{0}?key=99309f7a-5fee-4743-ae83-acb3a20a84f4";
            var resp = httpClient.GetAsync(string.Format(url, word)).Result;
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            
            try
            {
                var content = resp.Content.ReadAsAsync<string>(new MediaTypeFormatter[] { new WebsterFormatter() }).Result;
                return content;
            }
            catch (Exception)
            {
                return null;
            }

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
            // use regex match:  
//            Breakdown of regexp:
//^ Anchor at start of string
//[^\$]* Zero or more characters that are not $
//\$ Match a dollar sign
//[^\$]* Zero or more characters that are not $
//$ Anchor at end of string

            var result = possibleWord;
            foreach (char c in boardLetter.ToLower() + letterOnHand.ToLower())
            {
                result = ReplaceFirst(result, c.ToString(), string.Empty);
                
            }

            return result.Length == 0;
        }

        int CalculatePoint(string word, Bonus[] bonuses)
        {
            // regular point
            int point = 0;
            foreach (char c in word)
            {
                point += points[c];
            }

            // bonuses
            foreach (Bonus bonus in bonuses)
            {
                if (bonus.Position < word.Length)
                {
                    switch (bonus.Type)
                    {
                        case BonusType.DW:
                            point = point * 2;
                            break;
                        case BonusType.TW:
                            point = point * 3;
                            break;
                        case BonusType.DL:
                            point += points[word[bonus.Position]]; // 1x already taken into account
                            break;
                        case BonusType.TL:
                            point += points[word[bonus.Position]] * 2; // 1x already taken into account.  Adding the reminder 2x
                            break;
                    }
                }
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
        public Word GetWord(int id, [FromUri] Bonus bonuses = null)
        {
            //http://stackoverflow.com/questions/13962748/pass-array-of-an-object-to-webapi/13963155#13963155
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

    public class WebsterInfo
    {
        public string WavLink { get; set; }
        public string Definition { get; set; }
    }

    public class WebsterFormatter : XmlMediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }
        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, System.IO.Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task<object>.Factory.StartNew(() =>
            {
                var reader = new StreamReader(readStream);
                var text = reader.ReadToEnd();

                var regex = new Regex("<wav>[a-zA-Z0-9]*.wav</wav>");
                var wavLink = regex.Match(text).Value.Replace("<wav>", "").Replace("</wav>", "");
                if(string.IsNullOrEmpty(wavLink))
                {
                    wavLink  = "{wav}";
                }

                //<dt>:
                regex = new Regex("<dt>:[a-zA-Z0-9 '.s\"\\(\\):]*<");
                var def = regex.Match(text).Value.Replace("<dt>:", "").Replace("<", "");
                if(string.IsNullOrEmpty(def))
                {
                    def = "{def}";
                }

                return wavLink + ";" +def;
            });
        }
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }
        
    }
}