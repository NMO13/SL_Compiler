PROGRAM
VAR i, j:INTEGER;
BEGIN
i := 10;
j := 5;
i := i % j;
Assert(i, 0);

i := 2;
i := j % i;
Assert(i, 1);

i := 5;
i := i % 3;
Assert(i, 2);
END.