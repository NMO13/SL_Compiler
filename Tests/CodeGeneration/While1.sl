PROGRAM
VAR i, j:INTEGER;
BEGIN
    j := 0;
	i := 1;
	WHILE i < 5 DO
		j := j + 1;
		i := i + 1
	END;
	Assert(j, 4)
END.