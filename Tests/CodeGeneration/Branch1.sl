PROGRAM
VAR i, j:INTEGER;
BEGIN
i := 1;
j := 2;
	IF i < 3 THEN
		j := j + 21 / 7 + i * i
	ELSE
		j := 0
	END;
	Assert(j, 6);
	
	IF j + 21 / 7 + i * i >= j THEN
		Assert(1, 1)
	ELSE
		Assert(1, 0)
	END;
END.