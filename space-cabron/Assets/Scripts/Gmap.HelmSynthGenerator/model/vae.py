import tensorflow as tf


def get_encoder(version, input_size, latent_dim):
    if version == 1:
        return (
            tf.keras.Sequential([
                tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size / 2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size / 4, activation=tf.nn.relu),
                tf.keras.layers.Dense(latent_dim, activation=tf.nn.sigmoid),
            ]),
            tf.keras.Sequential([
                tf.keras.layers.Dense(latent_dim, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size / 4, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size / 2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size),
            ])
        )
    elif version == 2:
        return (
            tf.keras.Sequential([
                tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*4, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size/2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size/4, activation=tf.nn.relu),
                tf.keras.layers.Dense(latent_dim, activation=tf.nn.sigmoid),
            ]),
            tf.keras.Sequential([
                tf.keras.layers.Dense(latent_dim, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size/4, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size/2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*4, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
                tf.keras.layers.Dense(input_size),
            ])
        )


class VAE(tf.keras.Model):
    """Variational autoencoder."""

    def __init__(self, latent_dim, input_size, version=1):
        super(VAE, self).__init__()
        self.latent_dim = latent_dim
        self.input_size = input_size

        # (encoder, decoder) = get_encoder(version, input_size, latent_dim)

        self.encoder = tf.keras.Sequential([
            tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*4, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size/2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size/4, activation=tf.nn.relu),
            tf.keras.layers.Dense(latent_dim, activation=tf.nn.sigmoid),
        ])

        self.decoder = tf.keras.Sequential([
            tf.keras.layers.Dense(latent_dim, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size/4, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size/2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*4, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size*2, activation=tf.nn.relu),
            tf.keras.layers.Dense(input_size),
        ])

    @tf.function
    def sample(self, eps=None):
        if eps is None:
            eps = tf.random.normal(shape=(100, self.latent_dim))
        return self.decode(eps, apply_sigmoid=True)

    def encode(self, x):
        mean, logvar = tf.split(self.encoder(x), num_or_size_splits=2, axis=1)
        return mean, logvar

    def reparameterize(self, mean, logvar):
        eps = tf.random.normal(shape=mean.shape)
        return eps * tf.exp(logvar * 0.5) + mean

    def decode(self, z, apply_sigmoid=False):
        logits = self.decoder(z)
        if apply_sigmoid:
            probs = tf.sigmoid(logits)
            return probs
        return logits

    def compute_loss(self, x):
        mean, logvar = self.encode(x)
        z = self.reparameterize(mean, logvar)
        x_pred = self.decode(z)
        return tf.losses.mean_squared_error(x, x_pred)

    @tf.function
    def train_step(model, x, optimizer):
        """Executes one training step and returns the loss.

        This function computes the loss and gradients, and uses the latter to
        update the model's parameters.
        """
        with tf.GradientTape() as tape:
            loss = model.compute_loss(x)
        gradients = tape.gradient(loss, model.trainable_variables)
        optimizer.apply_gradients(zip(gradients, model.trainable_variables))
