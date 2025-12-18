using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WomenEmpower.API.DTOs;
using WomenEmpower.API.Services;
using WomenEmpower.Core.Entities;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager; // New
    private readonly TokenService _tokenService; // New

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService) // Injected
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTOs registerDTOs)
    {
        var user = new ApplicationUser
        {
            UserName = registerDTOs.Email, //user email as the username
            Email = registerDTOs.Email,
            OrganizationName = registerDTOs.OrganizationName,

        };

        //This securely hashes the password and save the user to the databse
        var result = await _userManager.CreateAsync(user, registerDTOs.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "Staff user Registered succesffully" });

    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login(UserLoginDTOs loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid Email or Password");

        // Check password hash against the database hash
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized("Invalid Email or Password");

        // Success! Issue the JWT token
        return Ok(new
        {
            Email = user.Email,
            Token = _tokenService.CreateToken(user),
            Organization = user.OrganizationName
        });
    }
}