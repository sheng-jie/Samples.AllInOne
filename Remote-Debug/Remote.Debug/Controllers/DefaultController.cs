using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Remote.Debug.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    class Symbol
    {
        public string Name { get; private set; } /*...*/
    }
    class Compiler
    {
        private List<Symbol> symbols;
        
        public Symbol FindMatchingSymbol(string name) 
        { 
            foreach (Symbol s in symbols)
            { 
                if (s.Name == name) 
                    return s; 
            } 
            return null; 
        }
    }

}