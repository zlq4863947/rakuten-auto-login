import * as path from 'path';

import { InputParams } from './types';

const edge = require('edge-js');

const modelPath = path.join(path.dirname(__filename), '../../dll');
const invokeFunc = edge.func({
  source: modelPath + '/marketspeed.cs',
  typeName: 'RakutenAutoLogin.MarketSpeed',
  methodName: 'GetInvoker',
});

/**
 * 乐天证券对象类
 */
export class MarketSpeed {
  private readonly invoke: any;

  /**
   * 构造函数
   * @param input 入力参数
   */
  constructor(input: InputParams) {
    this.invoke = invokeFunc(input);
  }

  login(): boolean {
    return this.invoke({ method: 'Login' }, true);
  }

  restart(): boolean {
    return this.invoke({ method: 'Restart' }, true);
  }

  exit(): void {
    return this.invoke({ method: 'Exit' }, true);
  }

  isRunning(): boolean {
    return this.invoke({ method: 'IsRunning' }, true);
  }

  isLogged(): boolean {
    return this.invoke({ method: 'IsLogged' }, true);
  }

  startRSS(): void {
    return this.invoke({ method: 'StartRSS' }, true);
  }

  restartRSS(): void {
    return this.invoke({ method: 'RestartRSS' }, true);
  }

  exitRSS(): void {
    return this.invoke({ method: 'ExitRSS' }, true);
  }

  isRunningRSS(): boolean {
    return this.invoke({ method: 'IsRunningRSS' }, true);
  }
}
