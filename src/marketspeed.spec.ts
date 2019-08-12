import { MarketSpeed } from './marketspeed';
import { InputType } from './types';

describe('楽天証券自動ログイン', () => {
  it('自動ログインテスト', async () => {
    const input: InputType = {
      user: 'idxxx',
      password: 'passxxx',
      version: '15.4',
      dir: 'C:/Program Files (x86)/MarketSpeed/MarketSpeed',
      filename: 'MarketSpeed.exe',
    };
    const marketSpeed = new MarketSpeed(input);
    const res = await marketSpeed.login();

    expect(res).toBeTruthy();
  });
});
