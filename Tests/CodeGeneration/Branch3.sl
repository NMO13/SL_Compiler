PROGRAM
VAR i, j:INTEGER;
BEGIN
i := 1;
j := 2;
	IF i > 3 THEN
		j := j + 21 / 7 + i * i
	ELSIF j = 2 THEN
		j := 0
	ELSIF j = 3 THEN
		j := 0;
	END;
END.