import * as React from "react";
import { FilePond, File } from "react-filepond";

import "filepond/dist/filepond.min.css";

class FileUpload extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      files: []
    };
  }

  render() {
    return (
      <div className="App">
        <FilePond
          ref={ref => (this.pond = ref)}
          allowMultiple={true}
          maxFiles={3}
          server="/api/upload"
          onupdatefiles={fileItems => {
            this.setState({
              files: fileItems.map(fileItem => fileItem.file)
            });
          }}
        >
          {this.state.files.map(file => (
            <File key={file} src={file} origin="local" />
          ))}
        </FilePond>
      </div>
    );
  }
}

export default FileUpload;
