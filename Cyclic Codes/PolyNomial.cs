using System;
using System.Collections.Generic;
using System.Linq;

namespace CyclicCodes
{
    class PolyNomial
    {
        List<int> pl;

        public PolyNomial()
        {
            pl = new List<int>();
        }

        public PolyNomial(int degree)
        {
            pl = new List<int>();
            for (var i = 0; i < degree; i++)
            {
                pl.Add(0);
            }
        }

        public PolyNomial(string m)
        {
            if (m.Contains("+") || m.Contains("X"))
            {
                var p = m.Replace(" ", "").Replace("1+", "0+").Replace("X+", "1+").Replace("X", "")
                    .Split(new char[] {'+'});

                if (p.Count() == 1 && p[0] == "")
                {
                    pl = new List<int>();
                    pl.Add(0);
                    pl.Add(1);
                    return;
                }

                var temp = p.Select(s => Convert.ToInt32(s)).ToList();

                pl = new List<int>();

                for (var i = 0; i < temp.Max() + 1; i++)
                {
                    pl.Add(0);
                }

                foreach (var i in temp)
                {
                    pl[i] = 1;
                }
            }
            else
            {
                pl = new List<int>();
                var p = m.Replace(" ", "");
                foreach (var c in p)
                {
                    pl.Add(c == '1' ? 1 : 0);
                }
            }
        }

        public int this[int r]
        {
            get => pl[r];
            set => pl[r] = value;
        }

        public PolyNomial Remainder(PolyNomial gX)
        {
            if (Length < gX.Length)
            {
                return this;
            }
            else
            {
                return AddWithoutCarry(gX.RightShift(Length - gX.Length)).Remainder(gX);
            }
        }

        public PolyNomial RightShift(int shiftBy)
        {
            var temp = new PolyNomial();
            temp.pl = new List<int>(pl.ToArray());
            temp.pl.InsertRange(0, new PolyNomial(shiftBy).pl);
            return temp;
        }

        public PolyNomial LeftShift(int shiftBy)
        {
            var temp = new PolyNomial();
            temp.pl = new List<int>(pl.ToArray());
            temp.pl.InsertRange(temp.pl.Count, new PolyNomial(shiftBy).pl);
            return temp;
        }

        public PolyNomial Join(PolyNomial p)
        {
            var temp = new PolyNomial(pl.Count + p.pl.Count);

            for (var i = 0; i < pl.Count; i++)
            {
                temp.pl[i] = pl[i];
            }

            for (var i = pl.Count; i < temp.pl.Count; i++)
            {
                temp.pl[i] = p.pl[i];
            }

            return temp;
        }

        public bool IsAllZero
        {
            get
            {
                foreach (var i in pl)
                {
                    if (i != 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string ToBinaryString()
        {
            var bs = "";
            foreach (var i in pl)
            {
                bs = bs + i.ToString();
            }

            return bs;
        }

        public string ToPolynomialString()
        {
            var ps = "";
            for (var i = 0; i < pl.Count; i++)
            {
                if (pl[i] == 1)
                {
                    if (i == 0)
                    {
                        ps = "1";
                    }
                    else if (i == 1)
                    {
                        ps = ps + "+X";
                    }
                    else
                    {
                        ps = ps + "+X" + i.ToString();
                    }
                }
            }

            return ps.Trim(new char[] {'+'});
        }

        public int Length => pl.Count;

        public PolyNomial AddWithoutCarry(PolyNomial p)
        {
            var temp = new PolyNomial(pl.Count);
            for (var i = 0; i < pl.Count; i++)
            {
                temp.pl[i] = pl[i] + p.pl[i];
                if (temp.pl[i] == 2)
                {
                    temp.pl[i] = 0;
                }
            }

            reStart:
            for (var i = temp.pl.Count - 1; i >= 0; i--)
            {
                if (temp.pl[i] == 0)
                {
                    temp.pl.RemoveAt(i);
                    goto reStart;
                }

                break;
            }

            return temp;
        }
    }
}