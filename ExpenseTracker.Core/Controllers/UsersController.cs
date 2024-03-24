// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions.Exceptions;
using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using ExpenseTracker.Core.Services.Foundations.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : RESTFulController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) =>
            this.userService = userService;

        //[HttpPost]
        //public async ValueTask<ActionResult<User>> PostUserAsync(User user, string password)
        //{
        //    try
        //    {
        //        User registeredUser = await this.userService.RegisterUserAsync(user, password);

        //        return Created(registeredUser);
        //    }
        //    catch (UserValidationException userValidationException)
        //    {
        //        return BadRequest(userValidationException.InnerException);
        //    }
        //    catch (UserDependencyValidationException userDependencyValidationException)
        //        when (userDependencyValidationException.InnerException is AlreadyExistsUserException)
        //    {
        //        return Conflict(userDependencyValidationException.InnerException);
        //    }
        //    catch (UserDependencyValidationException userDependencyValidationException)
        //        when (userDependencyValidationException.InnerException is InvalidUserReferenceException)
        //    {
        //        return FailedDependency(userDependencyValidationException.InnerException);
        //    }
        //    catch (UserDependencyException userDependencyException)
        //    {
        //        return InternalServerError(userDependencyException);
        //    }
        //    catch (UserServiceException userServiceException)
        //    {
        //        return InternalServerError(userServiceException);
        //    }
        //}

        [HttpGet]
        public ActionResult<IQueryable<User>> GetAllUsers()
        {
            try
            {
                IQueryable<User> retrievedUsers = this.userService.RetrieveAllUsers();

                return Ok(retrievedUsers);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException);
            }
        }

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<User>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                User user = await this.userService.RetrieveUserByIdAsync(userId);

                return Ok(user);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<User>> PutUserAsync(User user)
        {
            try
            {
                User modifiedUser =
                    await this.userService.ModifyUserAsync(user);

                return Ok(modifiedUser);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
                when (userDependencyValidationException.InnerException is LockedUserException)
            {
                return Locked(userDependencyValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
            {
                return BadRequest(userDependencyValidationException.InnerException);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException);
            }
        }

        [HttpDelete("{userId}")]
        public async ValueTask<ActionResult<User>> DeleteUserByIdAsync(Guid userId)
        {
            try
            {
                User deletedUser = await this.userService.RemoveUserByIdAsync(userId);

                return Ok(deletedUser);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
                when (userDependencyValidationException.InnerException is LockedUserException)
            {
                return Locked(userDependencyValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
            {
                return BadRequest(userDependencyValidationException);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException);
            }
        }
    }
}
