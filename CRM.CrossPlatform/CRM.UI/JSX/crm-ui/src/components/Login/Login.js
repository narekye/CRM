import React, { Component } from "react";

class Login extends Component {
  
  constructor(props) {
    super(props);
    this.getCredentials = this.getCredentials.bind(this);
    this.getToken = this.getToken.bind(this);
    this.username = '';
    this.password = '';
  }  

  getCredentials() {    
    this.username = this.refs['username'].value;
    this.password = this.refs['password'].value;
    this.getToken();
  }

  getToken() {
    let body = JSON.stringify({
        grant_type : 'password',
        username : this.username,
        password : this.password
    });
    console.log(body);
  }

  setToken() {

  }

  render() {
  return (<div> 
        <input ref='username' type='text' className='' placeholder='Username'></input>
        <br/>
        <input ref='password' type='password' className='' placeholder='Password'></input>
        <br/>
        <button onClick={this.getCredentials}> Login </button>
      </div>
      );
  }
}

export default Login;
