using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSPatients.Models;

namespace RSPatients.Controllers
{
    //Controller class for Medication to view list of Medication, create, edit, see details of each medication and delete
    public class RSMedicationController : Controller
    {
        private readonly PatientsContext _context;


        //Constructor of the class
        public RSMedicationController(PatientsContext context)
        {
            _context = context;
        }


        //Action to view the list of medication in the database
        // GET: RSMedication
        public async Task<IActionResult> Index(String id, String name)
        {
            if (String.IsNullOrWhiteSpace(id) || String.IsNullOrWhiteSpace(name))
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("code")))
                {
                    TempData["Error"] = "No medication type code available";

                    return RedirectToAction("Index", "RSMedicationType");

                }
                else
                {
                    id = HttpContext.Session.GetString("code");
                    name = HttpContext.Session.GetString("medName");

                }
            }
            else
            {
                HttpContext.Session.SetString("code", id);
                HttpContext.Session.SetString("medName", name);
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                name = _context.MedicationType.FirstOrDefault(c => c.MedicationTypeId == Convert.ToInt32(id)).Name;
            }
            ViewData["MedTypeName"] = name;

            var patientsContext = _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .Where(m => m.MedicationTypeId == Convert.ToInt32(id))
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Concentration);
            return View(await patientsContext.ToListAsync());
        }


        //Action to view the details of a medication 
        // GET: RSMedication/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["MedTypeName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }


        // Action to view the create page 
        // GET: RSMedication/Create
        public IActionResult Create()
        {
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(x=>x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(x=>x.DispensingCode), "DispensingCode", "DispensingCode");
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            ViewData["MedTypeName"]= HttpContext.Session.GetString("medName");
            return View();
        }


        //Action to create a new medication 
        // POST: RSMedication/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            medication.MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetString("code"));
            if (_context.Medication.Any(m => m.Name == medication.Name) && _context.Medication.Any(m => m.Concentration == medication.Concentration) && _context.Medication.Any(m => m.ConcentrationCode == medication.ConcentrationCode))
            {
                ModelState.AddModelError("Name", "Medication already exists");
            }
            if (ModelState.IsValid)
            {
                _context.Add(medication);
                await _context.SaveChangesAsync();
                TempData["Alert"] = "Created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(x=>x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(x=>x.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }


        // Action to view edit page
        // GET: RSMedication/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(x=>x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(x=>x.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            ViewData["MedTypeName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }


        // Action to edit a medication 
        // POST: RSMedication/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            medication.MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetString("code"));
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alert"] = "Update successful";

                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(x=>x.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(x=>x.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            ViewData["MedTypeName"]= HttpContext.Session.GetString("medName");
            return View(medication);
        }


        //Action to view the delete page 
        // GET: RSMedication/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["MedTypeName"] = HttpContext.Session.GetString("medName");
            return View(medication);
        }


        //Action to delete a medication 
        // POST: RSMedication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            ViewData["MedTypeName"] = HttpContext.Session.GetString("medName");
            TempData["Alert"] = "Record Deleted";
            return RedirectToAction(nameof(Index));
        }
        //Method to check if there are any duplicates
        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
