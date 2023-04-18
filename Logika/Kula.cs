﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logika
{
    public class Kula : IEquatable<Kula>
    {
        public int Srednica { get; init; }

        public Vector2 Szybkosc { get; set; }
        public Vector2 Pozycja { get; set; }


        public Kula(int srednica, Vector2 szybkosc, Vector2 pozycja)
        {
            Srednica = srednica;
            Szybkosc = szybkosc;
            Pozycja = pozycja;
        }

        public void Poruszanie(Vector2 granicaX, Vector2 granicaY)
        {

            if (Szybkosc.CzyZero())
            {
                return;
            }

            Pozycja += Szybkosc;

            var (pozX, pozY) = Pozycja;


            if (!pozX.IsBetween(granicaX.X-20, granicaX.Y, Srednica))
            {
                Szybkosc = new Vector2(-Szybkosc.X, Szybkosc.Y);
            }
            if (!pozY.IsBetween(granicaY.X - 20, granicaY.Y, Srednica))
            {
                Szybkosc = new Vector2(Szybkosc.X, -Szybkosc.Y);
            }

        }

        //wymagane do interfejsu equatable
        public bool Equals(Kula? other)
        {
            return other is not null
                && Srednica == other.Srednica
                && Pozycja == other.Pozycja
                && Szybkosc == other.Szybkosc;
        }

    }
}
