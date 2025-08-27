Notes on SQL design decisions
- CarQuote.IsCurrent and CarStatus.IsCurrent are used with filtered unique indexes to enforce "only one current".
- 'Picked Up' requiring a date should be enforced at application level or via trigger because CHECK constraints can't reference other tables.
- Normalized Make/Model/Submodel to avoid repeating strings.
- BuyerZipQuote stores buyer offers per zip; CarQuote materializes applicable quotes for each car (helps historical tracking).
