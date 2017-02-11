PROGRAM
VAR i, j, k:INTEGER;
BEGIN
i := 10;
j := 510;
j := i + j - 500; // big number
Assert(j, 20);

i := 500;
j := 400;
k := 100;
i := - i + j - k;
Assert(i, -200);
END.