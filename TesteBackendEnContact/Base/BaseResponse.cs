using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace TesteBackendEnContact.Base
{
    public class BaseResponse<TModel>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("response")]
        public TModel? Response { get; set; }

        public static BaseResponse<TModel> Ok(TModel model)
        {
            return new BaseResponse<TModel>
            {
                Code = StatusCodes.Status200OK,
                Response = model
            };
        }

        public static BaseResponse<TModel> Created(TModel model)
        {
            return new BaseResponse<TModel>
            {
                Code = StatusCodes.Status201Created,
                Response = model
            };
        }

        public static BaseResponse<TModel> Accepted(TModel model)
        {
            return new BaseResponse<TModel>
            {
                Code = StatusCodes.Status202Accepted,
                Response = model
            };
        }

        public static BaseResponse<TModel> NoContent()
        {
            return new BaseResponse<TModel>
            {
                Code = StatusCodes.Status204NoContent
            };
        }
    }
}