using System;
using Info.Blockchain.API.Wallet;
using Info.Blockchain.API.ExchangeRates;
using Info.Blockchain.API.PushTx;
using Info.Blockchain.API.Statistics;

namespace Info.Blockchain.API.Client
{
    public class BlockchainApiHelper : IDisposable
    {
        private readonly IHttpClient _baseHttpClient;
        private readonly IHttpClient _serviceHttpClient;
        public readonly BlockExplorer.BlockExplorer BlockExplorer;
        public readonly WalletCreator WalletCreator;
        public readonly TransactionPusher TransactionBroadcaster;
        public readonly ExchangeRateExplorer ExchangeRateExplorer;
        public readonly StatisticsExplorer StatisticsExplorer;

        public BlockchainApiHelper(string apiCode = null, IHttpClient baseHttpClient = null, string serviceUrl = null, IHttpClient serviceHttpClient = null)
        {
            if (baseHttpClient == null)
            {
                baseHttpClient = new BlockchainHttpClient(apiCode);
            }
            else
            {
                this._baseHttpClient = baseHttpClient;
                if (apiCode != null)
                {
                    baseHttpClient.ApiCode = apiCode;
                }
            }

            if (serviceHttpClient == null && serviceUrl != null)
            {
                serviceHttpClient = new BlockchainHttpClient(apiCode, serviceUrl);
            }
            else if (serviceHttpClient != null)
            {
                this._serviceHttpClient = serviceHttpClient;
                if (apiCode != null)
                {
                    serviceHttpClient.ApiCode = apiCode;
                }
            }
            else
            {
                serviceHttpClient = null;
            }

            this.BlockExplorer = new BlockExplorer.BlockExplorer(baseHttpClient);
            this.TransactionBroadcaster = new TransactionPusher(baseHttpClient);
            this.ExchangeRateExplorer = new ExchangeRateExplorer(baseHttpClient);
            this.StatisticsExplorer = new StatisticsExplorer(new BlockchainHttpClient("https://api.blockchain.info"));

            WalletCreator = serviceHttpClient != null ? new WalletCreator(serviceHttpClient) : null;

        }

        /// <summary>
        /// Creates an instance of 'WalletHelper' based on the identifier allowing the use
        /// of that wallet
        /// </summary>
        /// <param name="identifier">Wallet identifier (GUID)</param>
        /// <param name="password">Decryption password</param>
        /// <param name="secondPassword">Second password</param>
        public Wallet.Wallet InitializeWallet(string identifier, string password, string secondPassword = null)
        {
            if (_serviceHttpClient == null)
            {
                throw new ClientApiException("In order to create wallets, you must provide a valid service_url to BlockchainApiHelper");
            }
            return new Wallet.Wallet(_serviceHttpClient, identifier, password, secondPassword);
        }

        public WalletCreator CreateWalletCreator()
        {
            return new WalletCreator(_serviceHttpClient);
        }

        public void Dispose()
        {
            _baseHttpClient?.Dispose();
            _serviceHttpClient?.Dispose();
        }
    }
}