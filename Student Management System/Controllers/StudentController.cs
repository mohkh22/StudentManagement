using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using Studentmanage.Data.Data;
using Studentmanage.Entities.Models;

namespace Student_Management_System.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? Fgrade, string date = "Asc")
        {
            IQueryable<Studentmanage.Entities.Models.Student> students = _context.Students;

            if (search != null)
            {
                students = students.Where(x => x.Name.Contains(search));

            }

            if (Fgrade != null && Fgrade != "Grade")
            {
                students = students.Where(x => x.Grade == Fgrade);
            }

            if (date == "Desc")
            {
                students = students.OrderByDescending(x => x.EnrollmentDate);
            }
            else
            {
                students = students.OrderBy(x => x.EnrollmentDate);
            }


            ViewData["CurrentSearch"] = search;
            ViewData["CurrentGrade"] = Fgrade;
            ViewData["CurrentDate"] = date;
            return View(await students.ToListAsync());

        }

        [HttpGet]
        public IActionResult Create()
        {
            var student = new Studentmanage.Entities.Models.Student();

            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Studentmanage.Entities.Models.Student student)
        {
            try
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                return View(student);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);

            if (student == null)
            {
                return NotFound("Student not found with the given ID: " + id);
            }

            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Studentmanage.Entities.Models.Student student)
        {
            try
            {
                _context.Students.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Update changes. " + ex.Message);
                return View(student);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);

            if (student == null)
            {
                return NotFound("Student not found with the given ID: " + id);
            }

            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Studentmanage.Entities.Models.Student student)
        {
            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Remove changes. " + ex.Message);
                return View(student);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);

            if (student == null)
            {
                return NotFound("Student not found with the given ID: " + id);
            }

            return View(student);
        }

        public ActionResult ExportToExcel()
        {
            var students = _context.Students.ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // ✅ يعمل مع EPPlus 5.8

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Age";
                worksheet.Cells[1, 3].Value = "Grade";
                worksheet.Cells[1, 4].Value = "EnrollmentDate";

                int row = 2;
                foreach (var student in students)
                {
                    worksheet.Cells[row, 1].Value = student.Name;
                    worksheet.Cells[row, 2].Value = student.Age;
                    worksheet.Cells[row, 3].Value = student.Grade;
                    worksheet.Cells[row, 4].Value = student.EnrollmentDate.ToShortDateString();
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
            }
        }

        public ActionResult ExportToPdf()
        {
            var students = _context.Students.ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("قائمة الطلاب").FontSize(20).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        // رأس الجدول
                        table.Header(header =>
                        {
                            header.Cell().Text("Name").Bold();
                            header.Cell().Text("Age").Bold();
                            header.Cell().Text("Grade").Bold();
                            header.Cell().Text("EnrollmentDate").Bold();
                        });

                        // بيانات الطلاب
                        foreach (var s in students)
                        {
                            table.Cell().Text(s.Name);
                            table.Cell().Text(s.Age.ToString());
                            table.Cell().Text(s.Grade);
                            table.Cell().Text(s.EnrollmentDate.ToShortDateString());
                        }
                    });
                });
            });

            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0;

            return File(pdfStream, "application/pdf", "Students.pdf");
        }


    }
}
