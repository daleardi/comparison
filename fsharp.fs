// This is a F# rewrite of a fully refactored C# Clean Code example. Compare with C# code here: 
// http://www.codeproject.com/Articles/1083348/Csharp-BAD-PRACTICES-Learn-how-to-make-a-good-code 
// F# here: 
// http://FunctionalSoftware.NET/fsharp-rewrite-of-a-fully-refactored-csharp-clean-code-example-612 
module Discount = 
	type Year = int 
	type [<Measure>] percent 
	type Customer = Simple | Valuable | MostValuable
	 
	type AccountStatus = 
		| Registered of Customer * since:Year 
		| UnRegistered 

	let customerDiscount = function 
		| Simple -> 1<percent> 
		| Valuable -> 3<percent> 
		| MostValuable -> 5<percent>
		 
	let yearsDiscount = function 
		| years when years > 5 -> 5<percent> 
		| years 			   -> 1<percent> * years
		 
	let accountDiscount = function 
		| Registered(customer,years) -> customerDiscount customer, yearsDiscount years 
		| UnRegistered 				 -> 0<percent> 				 , 0<percent>
		 
	let asPercent p = 
		decimal(p) / 100.0m 
	
	let reducePriceBy discount price = 
		price - price * (asPercent discount) 
	
	let calculateDiscountedPrice account price = 
		let customerDiscount, yearsDiscount = accountDiscount account 
		price 
		|> reducePriceBy customerDiscount 
		|> reducePriceBy yearsDiscount 
	
open Discount 
let tests = 
	[
		calculateDiscountedPrice (Registered(MostValuable, 1)) 100.0m 
		calculateDiscountedPrice (Registered(Valuable, 6)) 100.0m 
		calculateDiscountedPrice (Registered(Simple, 1)) 100.0m 
		calculateDiscountedPrice UnRegistered 100.0m 
	] = [94.05000M; 92.15000m; 98.01000M; 100.0M] 
