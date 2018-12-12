import React, { Component } from "react";
import FileUpload from "./FileUpload";

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Upload a file to Amazon S3</h1>
        <FileUpload />
      </div>
    );
  }
}
