using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using IdentityService.Domain.Entities.Elasticsearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ElasticsearchClient _elasticClient;

        public SearchController(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient;
        }


        /// <summary>
        /// User searchbox kutusudur. Bu kutuya kullanıcı ismi yazınca o isimdeki kullanıcılar listelenir.İnstagramda birini aratmak gibi
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Arama kelimesi boş olamaz.");

            var response = await _elasticClient.SearchAsync<UserDocument>(s => s
                .Index("users")
                .IgnoreUnavailable() // : Eğer "users" indexi henüz oluşmadıysa hata verme, boş dön!
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.UserName)
                        .Query(keyword)
                        .Fuzziness(new Fuzziness("AUTO"))
                    )
                )
            );

            
            if (!response.IsValidResponse)
            {
                return StatusCode(500, new
                {
                    Message = "Arama sırasında bir hata oluştu.",
                    DebugInfo = response.DebugInformation, 
                    ElasticError = response.ElasticsearchServerError?.Error?.Reason
                });
            }

            var results = response.Documents.ToList();
            return Ok(results);
        }

        /// <summary>
        /// Admin paneli için sadece 'User' ve 'Owner' rolündeki kullanıcıları arar.
        /// Sadece Admin yetkisine sahip kullanıcılar erişebilir.
        /// </summary>
        [HttpGet("admin-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsersForAdmin([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Arama kelimesi boş olamaz.");

            var response = await _elasticClient.SearchAsync<UserDocument>(s => s
                .Index("users")
                .IgnoreUnavailable()
                .Query(q => q
                    .Bool(b => b
                        // Kullanıcı adında arama kelimesi geçmeli
                        .Must(m => m
                            .Match(match => match
                                .Field(f => f.UserName)
                                .Query(keyword)
                                .Fuzziness(new Fuzziness("AUTO"))
                            )
                        )
                        //  Rolü sadece "User" veya "Owner" olmalı
                        .Filter(f => f
                            .Terms(t => t
                                .Field("role.keyword")
                                .Terms(new TermsQueryField(new FieldValue[] { "User", "Owner" }))
                            )
                        )
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                return StatusCode(500, new
                {
                    Message = "Admin araması sırasında bir hata oluştu.",
                    DebugInfo = response.DebugInformation
                });
            }

            var results = response.Documents.ToList();
            return Ok(results);
        }
    }
}