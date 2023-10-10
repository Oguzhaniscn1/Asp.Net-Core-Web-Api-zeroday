﻿using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {

                var books = _manager.Book.GetAllBooks(false);
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
               .Book.GetOneBooksById(id,false);
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

                _manager.Book.CreateOneBook(book);
                _manager.Save();
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
                //check book?
                var entity = _manager.Book.GetOneBooksById(id, true);

                if (entity is null)
                {
                    return NotFound();//404
                }
                //check id
                if (id != book.Id)
                {
                    return BadRequest();//400
                }

                entity.Title = book.Title;
                entity.Price = book.Price;
                _manager.Save();
                return Ok(book);//olumlu dön

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
                var entity = _manager.Book.GetOneBooksById(id, false);

                if (entity is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id:{id} could not found."
                    });//404

                _manager.Book.DeleteOneBook(entity);
                _manager.Save();
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
                var entity = _manager.Book.GetOneBooksById(id, true);
                if (entity is null)
                {
                    return NotFound();//404
                }
                bookPatch.ApplyTo(entity);
                _manager.Book.Update(entity);
                return NoContent();//204

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }



        }

    }
}
