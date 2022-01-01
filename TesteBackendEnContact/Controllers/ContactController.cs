using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Contact;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    public class ContactController : BaseController
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactRepository _contactRepository;

        public ContactController(ILogger<ContactController> logger, IContactRepository contactRepository)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        [HttpPost("upload")]
        public async Task<BaseResponse<string>> Upload(IFormFile file)
        {
            var list = new List<ContactViewModel>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        int contactBookId = worksheet.Cells[row, 1].Value is null ? default(int) : int.Parse(worksheet.Cells[row, 1].Value.ToString().Trim());
                        int companyId = worksheet.Cells[row, 2].Value is null ? default(int) : int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
                        string name = worksheet.Cells[row, 3].Value is null ? string.Empty : worksheet.Cells[row, 3].Value.ToString().Trim();
                        string phone = worksheet.Cells[row, 4].Value is null ? string.Empty : worksheet.Cells[row, 4].Value.ToString().Trim();
                        string email = worksheet.Cells[row, 5].Value is null ? string.Empty : worksheet.Cells[row, 5].Value.ToString().Trim();
                        string address = worksheet.Cells[row, 6].Value is null ? string.Empty : worksheet.Cells[row, 6].Value.ToString().Trim();

                        list.Add(new ContactViewModel(default(int), contactBookId, companyId, name, phone, email, address));
                    }
                }
            }

            var errorList = await _contactRepository.ImportContactFile(list);

            if (errorList.Count <= 0)
                return BaseResponse<string>.Created("Import completed successfully.");
            else if (errorList.Count == list.Count)
                return BaseResponse<string>.Created("No records imported.");
            else if (errorList.Count > 0)
                return BaseResponse<string>.Created("Partially imported records.");
            else
                return BaseResponse<string>.Created("Import failed.");
        }

        [HttpGet("export")]
        public async Task<FileStreamResult> Export()
        {
            List<ContactViewModel> contactViewModel = await _contactRepository.ExportContactFile();

            var memoryStream = new MemoryStream();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Contacts");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "ContactBookId";
                worksheet.Cells[1, 3].Value = "CompanyId";
                worksheet.Cells[1, 4].Value = "Name";
                worksheet.Cells[1, 5].Value = "Phone";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Address";

                for (int i = 0; i < contactViewModel.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = contactViewModel[i].Id;
                    worksheet.Cells[i + 2, 2].Value = contactViewModel[i].ContactBookId;
                    worksheet.Cells[i + 2, 3].Value = contactViewModel[i].CompanyId;
                    worksheet.Cells[i + 2, 4].Value = contactViewModel[i].Name;
                    worksheet.Cells[i + 2, 5].Value = contactViewModel[i].Phone;
                    worksheet.Cells[i + 2, 6].Value = contactViewModel[i].Email;
                    worksheet.Cells[i + 2, 7].Value = contactViewModel[i].Address;
                }

                package.Save();
            }

            memoryStream.Position = 0;
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "contacts.xlsx";

            return File(memoryStream, contentType, fileName);
        }

        [HttpGet]
        public async Task<BaseResponse<PagedResponseModel<ContactViewModel>>> Get([FromQuery] ContactFilter filter)
        {
            PagedResponseModel<ContactViewModel> contacts = await _contactRepository.GetAllAsync(filter);
            return BaseResponse<PagedResponseModel<ContactViewModel>>.Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<BaseResponse<ContactViewModel>> Get(int id)
        {
            ContactViewModel contact = await _contactRepository.GetAsync(id);
            return BaseResponse<ContactViewModel>.Ok(contact);
        }
    }
}
