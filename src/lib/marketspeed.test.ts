import { MarketSpeed, InputType } from '../';
import * as assert from 'power-assert';

const testLogin = async (done: () => void) => {
  const input: InputType = {
    user: 'idxxx',
    password: 'passxxx',
    version: '15.4',
    dir: 'C:/Program Files (x86)/MarketSpeed/MarketSpeed',
    filename: 'MarketSpeed.exe'
  }
  const marketSpeed = new MarketSpeed(input);
  assert(await marketSpeed.login())
  done();
};

describe('楽天証券自動ログイン', () => {

  it('自動ログインテスト', function (done) {
    this.timeout(8000);
    testLogin(done);
  });

});
