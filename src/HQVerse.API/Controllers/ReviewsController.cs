using System.Security.Claims;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Reviews;
using HQVerse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQVerse.API.Controllers;

/// <summary>
/// Gerencia avaliações, comentários e curtidas de edições
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : BaseApiController
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null ? int.Parse(claim) : 0;
    }

    /// <summary>
    /// Lista avaliações de uma edição específica
    /// </summary>
    [HttpGet("issue/{issueId:int}")]
    [ProducesResponseType(typeof(PaginatedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ReviewDto>>> GetByIssue(
        int issueId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetByIssueIdAsync(issueId, paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista avaliações de um usuário específico
    /// </summary>
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(PaginatedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ReviewDto>>> GetByUser(
        int userId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetByUserIdAsync(userId, paginationParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém detalhes de uma avaliação
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetByIdAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    /// <summary>
    /// Cria uma nova avaliação (requer autenticação)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReviewDto>> Create(
        [FromBody] CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var review = await _reviewService.CreateAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    /// <summary>
    /// Atualiza uma avaliação (apenas o autor)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> Update(
        int id,
        [FromBody] CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var review = await _reviewService.UpdateAsync(id, userId, dto, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    /// <summary>
    /// Remove uma avaliação (apenas o autor)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var deleted = await _reviewService.DeleteAsync(id, userId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Adiciona um comentário em uma avaliação
    /// </summary>
    [HttpPost("{reviewId:int}/comments")]
    [Authorize]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> AddComment(
        int reviewId,
        [FromBody] CreateCommentDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var comment = await _reviewService.AddCommentAsync(reviewId, userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = reviewId }, comment);
    }

    /// <summary>
    /// Remove um comentário (apenas o autor)
    /// </summary>
    [HttpDelete("comments/{commentId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var deleted = await _reviewService.DeleteCommentAsync(commentId, userId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Alterna like em uma avaliação (curtir/descurtir)
    /// </summary>
    [HttpPost("{reviewId:int}/like")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleLike(int reviewId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var liked = await _reviewService.ToggleLikeAsync(reviewId, userId, cancellationToken);
        return Ok(new { Liked = liked });
    }
}