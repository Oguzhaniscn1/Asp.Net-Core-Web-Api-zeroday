using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {

                var books = _manager.BookService.GetAllBooks(false);
                return Ok(books);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)//idsi belirli kitabı getir idsi belirsiz ise 404 //FromRoute(Name ="id")] routeden gelecek değişken
        {
            try
            {
                var book = _manager
               .BookService.GetOneBookById(id, false);
               //.Find(x => x.Id == id); benim yöntemim
               //.Where(x => x.Id == id)
               //.SingleOrDefault(); //singordefault yani id si uyan varsa getir yoksa null döndür, aşağı bölümde ise null olma durumunu kontrol edip 404 dönüyoruz.

                if (book is null)
                {
                    return NotFound();//404
                }
                return Ok(book);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)//kitap ekleyeceğiz//request body -> [FromBody]
        {
            try
            {
                if (book is null)
                {
                    return BadRequest();//400 istediğimiz tarzda birşey değil
                }

                _manager.BookService.CreateOneBook(book);

                return StatusCode(201, book);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book)// tek kitap güncellemesi
        {

            try
            {

                if(book is null)
                    return BadRequest();//404

                //check book?

                _manager.BookService.UpdateOneBook(id, book, false);
                return NoContent();//204

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }




        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBooks([FromRoute(Name = "id")] int id)//bütün kitapları sil
        {
            try
            {
                var entity = _manager.BookService.GetOneBookById(id, false);

                _manager.BookService.DeleteOneBook(id, false);
                return NoContent();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                //checked
                var entity = _manager.BookService.GetOneBookById(id,true);
                if (entity is null)
                {
                    return NotFound();//404
                }
                bookPatch.ApplyTo(entity);
                _manager.BookService.UpdateOneBook(id,entity,false);
                return NoContent();//204

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }



        }

    }
}
