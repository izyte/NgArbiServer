import { AppCommonMethodsService } from '../api/svc/app-common-methods.service';
import { HttpClient } from '@angular/common/http';
import { TableBase } from '../api/svc/app-common.datatable';
import { TableRowBase }from '../api/svc/app-common.datarow';
import { ColumnInfo } from '../api/mod/app-column.model';


//TEMPLATE START
export class TABLE_CLASS extends TableBase {

  public rows:Array<TABLE_ROW_CLASS> = [];

  //TABLE_DECLARATIONS

  constructor(public http:HttpClient,public apiUrl:string, public tables:Array<any>, public apiCommon:AppCommonMethodsService) {
    super(http, apiUrl,tables,apiCommon);

    this.derivedTable = this;

    //CONSTRUCTOR_CALLS

    this.InitializeTable();

  }

  Add(row?:TABLE_ROW_CLASS):TABLE_ROW_CLASS
  {
    return super.Add(row);
  }

  NewRow():TABLE_ROW_CLASS{return new TABLE_ROW_CLASS();}
  GetRows():Array<TABLE_ROW_CLASS>{return this.rows;}
  public set currentRow(value:TABLE_ROW_CLASS){super.__currentRow(value);}
  public get currentRow():TABLE_ROW_CLASS{return super.__currentRow();}
  public TableLinks():Array<string>{return this._tableLinks;}
  public Links():Array<any>{return this._links;}
  public get dirtyRows():Array<TABLE_ROW_CLASS>{return super.__dirtyRows();}
  public get newRows():Array<TABLE_ROW_CLASS>{return super.__newRows();}


}

export class TABLE_ROW_CLASS extends TableRowBase{
	constructor(TABLE_ROW_PROPERTIES){
    super();

  }

  // Returs the table object where the row is a member of.
  public get Table():TABLE_CLASS{ return super._Table(); }

//TABLE_ROW_CONSTRUCTOR_CALLS
}
