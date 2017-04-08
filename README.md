# NumberSeries
Predicts the next number in a given sequence of numbers

Here are some guidelines that I am following in order to simplify ambiguous cases:
    
* RULE 1: A single pattern should find at least 3 values (ref RULE 4)
	Reduce the constraint to 2 values if another pattern is found in the series (eg splitting the series into 2, one pattern finds 3 values, another finds 2)
* RULE 2: How do we determine complexity of a pattern: common standard series, +1, -1, *2, /2, ^2, recursive level, or something else
	A pattern (eg. diffs) with all the same numbers (3,3,3,...) is simpler than a pattern such as (7,9,13,...), but the 2nd pattern may be discovered before the 1st.
    2, 5, 3, 6, 5, 8, 6, x
    So, it means we need to check multiple patterns before settling on a single one.
* RULE 3: What shoud the depth of recursion be? Keep it 2 for now.
* RULE 4: Should the rules be same for all levels of recursion? Heuristially, it is likely to find a simple standard series in a level 1 or 2 in a recursive call.
* RULE 5: Assume series starts at the 1st number for now. We can generalize this later.
* RULE 6: Should we consider a subset/subsequence of a standard series? For now, let's consider both.
* RULE 7: Are the basic opearations (+,-,*,/) in one direction enough to find a pattern. No! Because we are also matching standard series, so, we need to match 1,1/2,1/3... or 1/2,1/3,1/5,1/7... for eg,
	which becomes tricky unless we simply reverse the division. 


Types of patterns implemented:

* Standard series:
	(Natural):	1 2 3  4 5 6
	(Primes):	2 3 5 7 11
	etc...

* Standard series interleaved:
	1, 2, 9, 24, 120, 36
	In the aove series factorials and squares are interleaved: 2 24 120, 1 9 36

* Diffs:	
	1 4 7 10 13 16
	The above series has a constant diff of 3
	
* Diffs of Diffs:	
	2, 3, 6, 13, 28; 
	diff1: 1, 3, 7, 15
	diff2: 2, 4, 8, (16) => exponentials

* Interleaving: 
	1, 9, 0, 2, 7, 1, 3, 5, 2, 4, 3
	The following three series are inteleaved to form the above series:
	series1: 1 2 3 4 ...
	series2: 9 7 5 3 ...
	series3: 0 1 2 ...

* Padded: 
	7, 26, 63, 124	
	The above series is composed of padded cubes: 2^3-1, 3^3-1, 4^3-1, 5^3-1
	
	2, 3, 10, 15, 26
	The above series is composed of alternating padded squares: 1^1+1, 2^2-1, 3^2+1, 4^2-1, 5^2+1

* 2-group:
	2, 5, 3, 6, 5, 8, 6
	(2, 5), (3, 6), (5, 8), (6, x) => in each group a-b = 3

	3, 4.5, 2, 3, 7.5, 11.25, 6.2
	(3, 4.5), (2, 3), (7.5, 11.25), (6.2, x) => in each group a*1.5 = b

	2, 4, 12, 14, 42, 44
	primay grouping: (2, 4), (12, 14), (42, 44) => in each group a-b = 2
	secondary grouping: 2, (4, 12), (14, 42), (44, x) => in each group a*3 = b

* 2-group-interleaving

* 3-group: 
	2, 1, 3, 3, 2, 5, 9, 0, 9, 1, 9
	(2, 1, 3), (3, 2, 5), (9, 0, 9), (1, 9, x) => in each group a+b=c

	1, 8, 12, 2, 2, 3, 4, 2, 0.75, 4, 3
	(1, 8, 12), (2, 2, 3), (4, 2, 0.75), (4, 3, x) => in each group a^b*c=12

* 3-group-interleaving

* 4-group: 56, 89, 33, 21, 57, 88, 35, 19, 60, 85, 30
	(56, 89, 33, 21), (57, 88, 35, 19), (60, 85, 30, x) => in each group a+b+c+d=199

	5, 1, 2, 8, 8, 1, 0, 9, 9, 2, 3
	(5, 1, 2, 8), (8, 1, 0, 9), (9, 2, 3, x) => in each group a+b+c=d

	35, 5, 7, 12, 14, 2, 7, 9, 42, 6, 7
	(35, 5, 7, 12), (14, 2, 7, 9), (42, 6, 7, x) => in each group a=b*c, b+c=d

* 4-group-interleaving


TODO: Power series
TODO: Use TPL