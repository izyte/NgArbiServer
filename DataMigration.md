# Data Migration Requirement
### Required Fields for every single table in the repository
- Unique Numeric Key Field (Int64)
- Field names must start with the ` table code ` followed by an underscore ` _ ` then the actual field name (preferably using the pascal notation.
- Fields required for stamping and Change tracking 
  1. ` <tableCode>_Created ` will store the date/time when the record is created
  2. ` <tableCode>_CreatedBy ` will store the record creator's login id
  3. ` <tableCode>_UpdatedBy ` will store the date/time when the record is updated
  4. ` <tableCode>_UpdatedBy ` will store the login id of the person who updated the record
  
  
  Open [README.md](https://github.com/izyte/NgArbiServer/blob/master/README.md)
