PROGRAM
VAR i, j, k:INTEGER;
BEGIN
i := -4;
j := -6;
k := i + j;
Assert(k, -10);

i := 10;
j := 10;
j := j + 21 / 7 + i / i;
Assert(j, 14);

i := 10;
j := i + 1;
Assert(j, 11);
j := i - 1;
Assert(j, 9);

j := i + 500; // big number
Assert(j, 510);

i := i + j;
Assert(520, i);

j := -j - i;
Assert(j, -1030);
END.