using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PillarUtils.Data;
using PillarUtils.Models;
using System.Diagnostics;

namespace PillarUtils.Controllers
{
    public class ToolsController : Controller
    {
        public enum ExcelImportMode
        {
            ClientImport,
            ContactImport,
            ArchiveItemImport
        }

        private readonly ApplicationDbContext _context;

        public ToolsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("ImportExcel");
        }

        public async Task<IActionResult> ImportExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile ExcelFile, int ImportModeInt)
        {
            Debug.WriteLine("UPLOADING!!!!!!!!!!!");
            Debug.WriteLine(ImportModeInt);
            //make sure there's 'there' there
            if (ExcelFile == null || ExcelFile.Length == 0)
                return BadRequest("Invalid or Empty File Uploaded");
            if (!ExcelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file type. Only .xlsx is supported.");

            //make sure we have a valid ImportMode
            var ImportMode = (ExcelImportMode)ImportModeInt;

            //copy data from the file into the stream
            using var stream = new MemoryStream();
            await ExcelFile.CopyToAsync(stream);
            stream.Position = 0; // Reset position for ClosedXML

            // Use ClosedXML to read the workbook
            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                return BadRequest("No worksheet found in Excel file.");

            for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                switch (ImportMode)
                {
                    case ExcelImportMode.ClientImport: //disabled for safety
                        //var clientName = worksheet.Cell(row, 1).GetValue<string>();
                        //var clientCode = worksheet.Cell(row, 2).GetValue<string>();
                        //Client client = new Client();
                        //client.Name = clientName;
                        //client.ClientCode = clientCode;
                        //// Create and add to database a Client with the proper data
                        //if (ModelState.IsValid)
                        //{
                        //    _context.Add(client);
                        //    await _context.SaveChangesAsync();
                        //}
                        //Debug.WriteLine($"Client with: {clientName} | {clientCode} Added!");
                        break;

                    case ExcelImportMode.ContactImport:
                        //var avazaID = worksheet.Cell(row, 1).GetValue<string>();
                        //var firstName = worksheet.Cell(row, 2).GetValue<string>();
                        //var lastName = worksheet.Cell(row, 3).GetValue<string>();
                        //var positionTitle = worksheet.Cell(row, 4).GetValue<string>();
                        //var email = worksheet.Cell(row, 5).GetValue<string>();
                        //var mobilePhone = worksheet.Cell(row, 6).GetValue<string>();
                        //var phoneNumber = worksheet.Cell(row, 7).GetValue<string>();
                        //var customerCode = worksheet.Cell(row, 8).GetValue<string>();
                        //var billingAddress = worksheet.Cell(row, 10).GetValue<string>();
                        ////Find Client by Client Code
                        //Client contactClient = await _context.Client.FirstOrDefaultAsync(c => c.ClientCode == customerCode);

                        //if (contactClient == null)
                        //{
                        //    Debug.WriteLine($"Client with code {customerCode} not found.");
                        //    continue; // or continue; depends on how you want to handle missing clients
                        //}

                        //// Create contact
                        //Contact contact = new()
                        //{
                        //    FirstName = firstName,
                        //    LastName = lastName,
                        //    AvazaUserId = avazaID,
                        //    JobTitle = positionTitle,
                        //    Email = email,
                        //    MobilePhone = mobilePhone,
                        //    WorkPhone = phoneNumber,
                        //    BillingAddress = billingAddress,
                        //    Client = contactClient // you can also just set ClientId = contactClient.Id;
                        //};

                        //_context.Add(contact);
                        //await _context.SaveChangesAsync();

                        //// TODO: Save to DB
                        //Debug.WriteLine($"Contact with : {firstName} {lastName} | {contactClient.ClientCode} Added!!!!!!");
                        break;

                    case ExcelImportMode.ArchiveItemImport:
                        var archiveTitle = worksheet.Cell(row, 1).GetValue<string>();
                        // TODO: Save to DB
                        Debug.WriteLine($"Archive: {archiveTitle}");
                        break;

                    default:
                        return BadRequest("Unknown import mode.");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
