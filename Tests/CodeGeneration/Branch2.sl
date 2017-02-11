PROGRAM
VAR i, j:INTEGER;
BEGIN
i := 1;
j := 2;
	IF i > 3 THEN
		j := j + 21 / 7 + i * i
	ELSIF j = 2 THEN
		j := 0
	END;
	Assert(j, 0);
	
	IF i # 1 THEN
		Assert(1, 0)
	ELSE
		Assert(1, 1)
	END;
END.