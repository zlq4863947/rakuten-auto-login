import * as path from 'path';

import { InputType } from './types';
const edge = require('edge-js');

const modelPath = path.join(path.dirname(__filename), '../../dll');
const invokeLogin = edge.func({
  source: modelPath + '/marketspeed.cs',
  typeName: 'RakutenAutoLogin.MarketSpeed',
  methodName: 'Login',
});

/**
 * 乐天证券对象类
 */
export class MarketSpeed {
  input: InputType;

  /**
   * 构造函数
   * @param input 入力参数
   */
  constructor(input: InputType) {
    this.input = input;
  }

  login() {
    return new Promise((resolve, reject) => {
      setTimeout(() => reject(new Error('cannnot login in 3000ms.')), 3000);
      return invokeLogin(this.input, (error: Error, result: any) => {
        return error ? reject(error) : resolve(result);
      });
    });
  }
}
